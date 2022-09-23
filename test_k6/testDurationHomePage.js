import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
  thresholds: {
    "http_req_duration": [{
      threshold: "p(95)<200"
    }],
    "checks": [{
      threshold: "rate>0.9"
    }],
    "http_req_failed": ["rate<0.1"]
  },
  scenarios: {
    contacts: {
      // https://k6.io/docs/using-k6/scenarios/executors/ how the interaction are distributed between the VUs
      executor: 'ramping-vus',// as many interactios as possible in the time defined
      startVUs: 0,
      // number of VUs to ramp up or down eg. in the first 2 sec going to 5 the after 3s 9 then ...
      stages: [
        { duration: '2s', target: 5 },
        { duration: '3s', target: 9 },
        { duration: '3s', target: 5 },
        { duration: '2s', target: 1 }
      ]
    }
  }

};

export default function () {

  let result = http.get('https://tutorial-reactivities.herokuapp.com/');

  check(result, {
    "Status is 200": (r) => r.status == 200,
    "Duration < 500ms": (r) => r.timings.duration < 500
  });

  sleep(1);
}