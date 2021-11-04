import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios';
import {Header} from 'semantic-ui-react';
import { List } from 'semantic-ui-react'
function App() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    axios.get('http://localhost:5000/api/activities').then( response => {
    console.log(response);  
    setActivities(response.data);
    })
  }, [])// we have added the dependecies list because otherwise is going to run constantly
  // every time something is changing in the component there is going to be a re render
  // and when we have a rerender we are going to call the useEffect methot and it's going to be a loop




  return (
    <div >
      <Header as='h2' icon='users' content='Reactivities'/>
      
        <List>
          {activities.map( (activity: any) =>(
            <List.Item key={activity.id}>
              {activity.title}
            </List.Item>
          ))}
        </List>
    </div>
  );
}

export default App;
