import React, { Fragment, useEffect, useState } from 'react';
import { Container } from 'semantic-ui-react'
import { Activity } from '../models/activity';
import NavBar from './NavBar';
import './styles.css'
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import {v4 as uuid} from 'uuid'
import agent from '../api/agent';
import LoadingComponent from './LoadingComponents';

function App() {
  // since we have added our typesctipt obj we can add the generic in use state
  // <Type>(initialValue)
  const [activities, setActivities] = useState<Activity[]>([]);
  const [selectedActivity, setSelectedActivity] = useState<Activity | undefined >(undefined);// | union of type
  const [editMode, setEditMode] = useState(false);// autoinference of type
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] =useState(false);
  useEffect(() => {
    agent.Activities.list().then( response => {
      response = response.map(activity => {
        activity.date = activity.date.split('T')[0];
        return activity;
      });
      setActivities(response);
      setLoading(false);
    })
  }, [])// we have added the dependecies list because otherwise is going to run constantly
  // every time something is changing in the component there is going to be a re render
  // and when we have a rerender we are going to call the useEffect methot and it's going to be a loop

  function handleSelectActivity(id:string){
    setSelectedActivity(activities.find(x => x.id === id))
  }
  function handleCancelSelectActivity(){
    setSelectedActivity(undefined);
  }

  function handleFormOpen(id?:string){
    id ? handleSelectActivity(id) : handleCancelSelectActivity();
    setEditMode(true);
  }
  function handleFormClose(){
    setEditMode(false);
  }
  // when we create oo modify an activity we call this function, we have decided to stre it here 
  // because it's also where we have the list of activities
  function handleCreateOrEditActivity(activity: Activity) {
    setSubmitting(true);
    if(activity.id)
      agent.Activities.update(activity).then(() =>{   // update db
        setActivities([...activities.filter(x => x.id !== activity.id), activity]); //update gui
        setSubmitting(false);    
        setEditMode(false);
        setSelectedActivity(activity);
      });
    else{
      activity.id = uuid();
      agent.Activities.create(activity).then(() =>{
        setActivities([...activities, activity]);
        setSubmitting(false);    
        setEditMode(false);
        setSelectedActivity(activity);
      });
    }
    
    
  }
  function handleDeleteActivity(id: string){
    setSubmitting(true);
    agent.Activities.delete(id).then(() => {
      setActivities([...activities.filter(x => x.id !== id)]);
      setSubmitting(false);
    })
  }
  if (loading)  return <LoadingComponent />
  return (
    <>
      <NavBar openForm = {handleFormOpen}/>
      <Container style={{marginTop:'7em'}}>
        <ActivityDashboard 
          activities={activities}
          selectedActivity = {selectedActivity}
          selectActivity = {handleSelectActivity}
          cancelSelectActivity = {handleCancelSelectActivity}
          editMode = {editMode}
          openForm = {handleFormOpen}
          closeForm = {handleFormClose}
          createOrEdit={handleCreateOrEditActivity}
          deleteActivity={handleDeleteActivity}
          submitting={submitting}
        />
      </Container>
    </>
  );
}

export default App;
