import {  makeAutoObservable, reaction, runInAction} from "mobx";
import agent from "../api/agent";
import { Activity, ActivityFormValues } from "../models/activity";
import {format} from 'date-fns';
import { store } from "./store";
import { Profile } from "../models/profile";
import { Pagination, PagingParams } from "../models/pagination";

export default class ActivityStore{
    activityRegistry = new Map<string, Activity>();
    selectedActivity: Activity | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);


    constructor(){
        makeAutoObservable(this);
        reaction(
            () => this.predicate.keys(),
            () => {
                this.pagingParams = new PagingParams();
                this.activityRegistry.clear();
                this.loadActivities();
            }
        )
    }

    setPagingParams = (pagingParams: PagingParams) =>{
        this.pagingParams = pagingParams;
    }

    setPredicate = (predicate: string, value: string | Date) => {
        const resetPredicate = () => {
            this.predicate.forEach((value, key) => {
                if (key !== 'startDate') this.predicate.delete(key);
            })
        }
        switch (predicate) {
            case 'all':
                resetPredicate();
                this.predicate.set('all', true);
                break;
            case 'isGoing':
                resetPredicate();
                this.predicate.set('isGoing', true);
                break;
            case 'isHost':
                resetPredicate();
                this.predicate.set('isHost', true);
                break;
            case 'startDate':
                this.predicate.delete('startDate');
                this.predicate.set('startDate', value);
        }
    } 

    get axiosParam(){
        // it's going to add all the paramiter at the query string
        const params = new URLSearchParams();
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => {
            if (key === 'startDate') {
                params.append(key, (value as Date).toISOString())
            } else {
                params.append(key, value);
            }
        });
        return params;
    }
    get activitiesByDate() {
        return Array.from(this.activityRegistry.values()).sort((a, b) => 
            a.date!.getTime() - b.date!.getTime());
    }

    get groupedActivities(){
        return Object.entries(
            this.activitiesByDate.reduce((activities, activity) => {
                const date =format(activity.date!,'dd MMM yyyy');
                activities[date] = activities[date] ? [...activities[date], activity] : [activity];
                return activities;
            }, {} as {[key: string]: Activity[]})
        )
    }



    loadActivities = async () => {
        this.setLoadingInitial(true);
        try {
            const res = await agent.Activities.list(this.axiosParam);
            res.data.forEach(activity => {
                this.setActivity(activity);
            });
            this.setPagination(res.pagination);
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);

        }
    }
    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }
    loadActivity =   async (id: string) => {
        let activity = this.activityRegistry.get(id);
        if(activity){
            // in this case we found it in memory
            this.selectedActivity = activity;
            return activity;
        }else{
            this.loadingInitial = true;
            try {
                
                activity = await agent.Activities.details(id);
                // we want to add it in memory
                this.setActivity(activity);
                this.setLoadingInitial(false);
                this.selectedActivity = activity;
                return activity;

            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
        //we want to check if we have the activity in memory
        // if not we are going to use the api to access the database
    }
    
    // add activity in the regestry and set information about user partecipation
    private setActivity = (activity: Activity) => {
        const user = store.userStore.user;
        if (user) {
            activity.isGoing = activity.attendees!.some(
                a => a.username === user.username
            )
            activity.isHost = activity.hostUsername === user.username;
            activity.host = activity.attendees?.find(x => x.username === activity.hostUsername);
        }
        activity.date = new Date(activity.date!)
        this.activityRegistry.set(activity.id, activity);
    }
    // to remove the warning changing (observed) observable values without using an action is not allowed
    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }
  
    
    createActivity = async (activity: ActivityFormValues) => {
        const user = store.userStore.user;
        const attendee = new Profile(user!);
        try {
            // save in db new activity
            await agent.Activities.create(activity);
            //update ojs for front end
            const newActivity = new Activity(activity);
            newActivity.hostUsername = user!.username;
            newActivity.attendees = [attendee];
            this.setActivity(newActivity);
            runInAction(() => {
                this.selectedActivity = newActivity;
            })
        } catch (error) {
            console.log(error);
        }
    }
    updateActivity = async (activity: ActivityFormValues) => {
        try {
            await agent.Activities.update(activity);
            runInAction(() => {
                if (activity.id) {
                    let updatedActivity = {...this.activityRegistry.get(activity.id), ...activity}
                    this.activityRegistry.set(activity.id, updatedActivity as Activity);
                    this.selectedActivity = updatedActivity as Activity;
                    this.selectedActivity = activity as Activity;

                }
            })
        } catch (error) {
            console.log(error);
        }
    }
    deleteActivity = async (id: string) => {
        this.loading = true;
        try {
            await agent.Activities.delete(id);
            runInAction(() => {
                this.activityRegistry.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }


    updateAttendance = async () => {
        const user = store.userStore.user;
        this.loading = true;
        try {

            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                // we want to remove the user
                if (this.selectedActivity?.isGoing) {
                    this.selectedActivity.attendees = 
                        this.selectedActivity.attendees?.filter(a => a.username !== user?.username);
                    this.selectedActivity.isGoing = false;
                } else {
                    // we want to add the user to the attendees
                    const attendee = new Profile(user!);
                    this.selectedActivity?.attendees?.push(attendee);
                    this.selectedActivity!.isGoing = true;
                }
                // we update also the object for frontend
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!)
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }
    cancelActivityToggle = async () => {
        this.loading = true;
        try {
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                this.selectedActivity!.isCancelled = !this.selectedActivity?.isCancelled;
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }
    clearSelectedActivity = () => {
        this.selectedActivity = undefined;
    }
    updateAttendeeFollowing = (username: string) => {
        this.activityRegistry.forEach(activity => {
            activity.attendees.forEach(attendee => {
                if (attendee.username === username) {
                    attendee.following ? attendee.followersCount-- : attendee.followersCount++;
                    attendee.following = !attendee.following;
                }
            })
        })
    }
} 
