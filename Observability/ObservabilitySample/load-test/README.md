# Load Testing with K6

This directory contains K6 load tests to stress test your observability setup.

## Prerequisites

### Install K6

**Windows (Chocolatey):**
```powershell
choco install k6
```

**Windows (Winget):**
```powershell
winget install k6
```

**Manual Download:**
Download from https://k6.io/docs/get-started/installation/

## Test Files

### 1. `load-test.js` - Full Load Test
- **Duration**: 60 seconds
- **Rate**: 50 requests per second
- **Total Requests**: ~3000
- **Features**:
  - Unique partner ID for every request
  - 80% success requests (`/call`)
  - 20% failure requests (`/nocall`)
  - Performance thresholds
  - Detailed logging

### 2. `quick-load-test.js` - Quick Test
- **Duration**: 10 seconds
- **Rate**: 50 requests per second  
- **Total Requests**: ~500
- **Purpose**: Quick validation

## How to Run

### Method 1: Using PowerShell Script (Recommended)
```powershell
.\Run-LoadTest.ps1
```
This script will:
- Check if K6 is installed
- Verify your service is running
- Show test configuration
- Run the full load test

### Method 2: Direct K6 Commands

**Full test:**
```powershell
k6 run load-test.js
```

**Quick test:**
```powershell
k6 run quick-load-test.js
```

## What to Monitor During Test

### Grafana Dashboard (http://localhost:3001)
- **Requests Per Second**: Should show ~50 RPS
- **QOS**: Should show ~80% success rate
- **Response Times**: Check latency percentiles
- **Error Rate**: Should see ~20% errors

### Prometheus (http://localhost:9090)
- `http_requests_total` - Total request count
- `http_request_duration_seconds` - Response time metrics

### Loki Logs
- Filter by different partner names
- See both success and failure logs
- Verify log volume matches request rate

## Expected Results

After running the full test, you should see:
- **~3000 total requests** in metrics
- **~2400 successful requests** (80%)
- **~600 failed requests** (20%)
- **All with unique partner IDs**
- **Rich logs in Loki**
- **Detailed metrics in Grafana**

## Customization

Edit `load-test.js` to modify:
- **Request rate**: Change `rate: 50` 
- **Duration**: Change `duration: '60s'`
- **Failure rate**: Change `Math.random() < 0.2` (currently 20%)
- **Endpoints**: Modify URL patterns
- **Thresholds**: Adjust performance requirements

## Troubleshooting

### "Service not available"
Start the observability stack first:
```powershell
# From the parent directory
..\Start-ObservabilityStack.ps1
```

### "K6 command not found"
Install K6 using one of the methods above.

### No metrics showing
- Wait 10-15 seconds for metrics to appear
- Check that Promtail container ID is updated
- Verify all containers are running: `docker ps`

## Running from Parent Directory

If you want to run the load test from the parent directory:
```powershell
# Change to load-test directory
cd load-test

# Run the test
.\Run-LoadTest.ps1

# Or run directly
k6 run load-test.js
```
