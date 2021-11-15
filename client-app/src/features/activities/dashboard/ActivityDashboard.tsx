import { observer } from "mobx-react-lite";
import React, { useEffect } from "react";
import { Grid } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponents";
import { useStore } from "../../../app/stores/store";
import ActivityFilters from "./ActivityFilter";
import ActivityList from "./ActivityList";

export default observer(function ActivityDashboard(){
    const {activityStore} = useStore(); // we destructure thr object to take just the part we are interested in
    const {loadActivities, activityRegistry, loadingInitial} = activityStore;


    // since we have added our typesctipt obj we can add the generic in use state
    // <Type>(initialValue)

    useEffect(() => {
        if (activityRegistry.size <= 1) loadActivities();
    }, [activityRegistry.size, loadActivities])// we have added the dependecies list because otherwise is going to run constantly
    // every time something is changing in the component there is going to be a re render
    // and when we have a rerender we are going to call the useEffect methot and it's going to be a loop

    // when we create oo modify an activity we call this function, we have decided to stre it here 
    // because it's also where we have the list of activities


    if (loadingInitial)  return <LoadingComponent content='Loading activities...' />
    return(
        <Grid>
            <Grid.Column width='10'>
                <ActivityList />
            </Grid.Column>
            <Grid.Column width='6'>
                <ActivityFilters/>                
            </Grid.Column>
        </Grid>
    )
})