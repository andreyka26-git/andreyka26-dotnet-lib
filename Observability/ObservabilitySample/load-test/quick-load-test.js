import http from 'k6/http';
import { check } from 'k6';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

// Quick test configuration - 10 seconds only
export const options = {
  scenarios: {
    quick_test: {
      executor: 'constant-arrival-rate',
      rate: 50, // 50 requests per second
      timeUnit: '1s',
      duration: '10s', // Run for 10 seconds only
      preAllocatedVUs: 5,
      maxVUs: 20,
    },
  },
};

const BASE_URL = 'http://localhost:3000';

export default function () {
  // Generate unique partner name
  const partnerId = `test_${__VU}_${__ITER}_${Date.now()}_${randomString(4)}`;
  
  // 80% success, 20% failure
  const shouldFail = Math.random() < 0.2;
  
  let url, expectedStatus;
  if (shouldFail) {
    url = `${BASE_URL}/nocall?partner=${partnerId}`;
    expectedStatus = 404;
  } else {
    url = `${BASE_URL}/call?partner=${partnerId}`;
    expectedStatus = 200;
  }
  
  const response = http.get(url);
  
  check(response, {
    'status is correct': (r) => r.status === expectedStatus,
    'response time < 3s': (r) => r.timings.duration < 3000,
  });
  
  console.log(`${partnerId} - ${response.status} - ${response.timings.duration}ms`);
}

export function setup() {
  console.log('🚀 Quick K6 Test - 50 RPS for 10 seconds (~500 requests)');
  return {};
}
