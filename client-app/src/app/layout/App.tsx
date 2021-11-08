import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { Container } from 'semantic-ui-react'
import { Activity } from '../models/activity';
import NavBar from './NavBar';
import './styles.css'
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import {v4 as uuid} from 'uuid'

function App() {
  // since we have added our typesctipt obj we can add the generic in use state
  // <Type>(initialValue)
  const [activities, setActivities] = useState<Activity[]>([]);
  const [selectedActivity, setSelectedActivity] = useState<Activity | undefined >(undefined);// | union of type
  const [editMode, setEditMode] = useState(false);// autoinference of type

  useEffect(() => {
    axios.get<Activity[]>('http://localhost:5000/api/activities').then( response => {
    setActivities(response.data);
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
    // the activities is an array we need to replace just one activity
    activity.id 
      ? setActivities([...activities.filter(x => x.id !== activity.id), activity])
      : setActivities([...activities, {...activity, id: uuid() }]);
    setEditMode(false);
    setSelectedActivity(activity);
  }
  function handleDeleteActivity(id: string){
    setActivities([...activities.filter(x => x.id !== id)])
  }

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
        />
      </Container>
    </>
  );
}

export default App;
