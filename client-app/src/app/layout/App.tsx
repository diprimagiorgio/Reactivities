import React, {  useEffect } from 'react';
import { Container } from 'semantic-ui-react'
import NavBar from './NavBar';
import './styles.css'
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import LoadingComponent from './LoadingComponents';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';

function App() {
  const {activityStore} = useStore(); // we destructure thr object to take just the part we are interested in



  // since we have added our typesctipt obj we can add the generic in use state
  // <Type>(initialValue)
  
  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore])// we have added the dependecies list because otherwise is going to run constantly
  // every time something is changing in the component there is going to be a re render
  // and when we have a rerender we are going to call the useEffect methot and it's going to be a loop

  // when we create oo modify an activity we call this function, we have decided to stre it here 
  // because it's also where we have the list of activities
  
  
  if (activityStore.loadingInitial)  return <LoadingComponent />
  return (
    <>
      <NavBar />
      <Container style={{marginTop:'7em'}}>
      <ActivityDashboard/>
      </Container>
    </>
  );
}

export default observer(App);
