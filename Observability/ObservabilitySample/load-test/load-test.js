import http from 'k6/http';
import { check, sleep } from 'k6';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

// Test configuration
export const options = {
  scenarios: {
    load_test: {
      executor: 'constant-arrival-rate',
      rate: 50, // 50 requests per second
      timeUnit: '1s',
      duration: '60s', // Run for 1 minute
      preAllocatedVUs: 10, // Pre-allocate 10 virtual users
      maxVUs: 100, // Max 100 virtual users if needed
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<2000'], // 95% of requests must complete within 2s
    http_req_failed: ['rate<0.1'], // Error rate must be less than 10%
  },
};

// Base URL for the service
const BASE_URL = 'http://localhost:3000';

// Counter for unique partner generation
let requestCounter = 0;

export default function () {
  // Generate unique partner name
  const partnerId = `partner_${__VU}_${__ITER}_${Date.now()}_${randomString(6)}`;
  
  // Randomly choose between success and failure endpoints
  const shouldFail = Math.random() < 0.2; // 20% failure rate
  
  let url, expectedStatus;
  
  if (shouldFail) {
    // Use /nocall endpoint to generate failures
    url = `${BASE_URL}/nocall?partner=${partnerId}`;
    expectedStatus = 404;
  } else {
    // Use /call endpoint for success
    url = `${BASE_URL}/call?partner=${partnerId}`;
    expectedStatus = 200;
  }
  
  // Make the request
  const response = http.get(url);
  
  // Validate response
  check(response, {
    'status is correct': (r) => r.status === expectedStatus,
    'response time < 5s': (r) => r.timings.duration < 5000,
    'response has body': (r) => r.body.length > 0,
    'response contains partner': (r) => r.body.includes(partnerId),
  });
  
  // Log every 100th request for monitoring
  if (__ITER % 100 === 0) {
    console.log(`VU ${__VU} - Iteration ${__ITER} - Partner: ${partnerId} - Status: ${response.status} - Duration: ${response.timings.duration}ms`);
  }
}

// Setup function runs once before the test
export function setup() {
  console.log('🚀 Starting K6 Load Test');
  console.log('📊 Target: 50 requests/second for 60 seconds');
  console.log('🎯 Total expected requests: ~3000');
  console.log('⚡ Each request uses unique partner ID');
  console.log('🔥 20% of requests will hit /nocall (failures)');
  console.log('✅ 80% of requests will hit /call (success)');
  
  // Test that the service is available
  const healthCheck = http.get(`${BASE_URL}/call?partner=healthcheck`);
  if (healthCheck.status !== 200) {
    throw new Error(`Service not available. Status: ${healthCheck.status}`);
  }
  
  console.log('✅ Service health check passed');
  return {};
}

// Teardown function runs once after the test
export function teardown(data) {
  console.log('🏁 Load test completed');
  console.log('📈 Check your Grafana dashboard for metrics');
  console.log('📝 Check Loki for logs from all the unique partners');
}
