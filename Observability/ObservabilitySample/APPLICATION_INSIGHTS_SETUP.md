# Application Insights Setup Instructions

This guide explains how to configure Application Insights to collect metrics and logs from the `/azure/call` endpoint.

## Prerequisites

1. An Azure subscription
2. An Application Insights resource in Azure

## Step 1: Create Application Insights Resource

1. **Login to Azure Portal**: Go to [https://portal.azure.com](https://portal.azure.com)

2. **Create Application Insights**:
   - Click "Create a resource"
   - Search for "Application Insights"
   - Click "Create"
   - Fill in the required information:
     - **Subscription**: Select your Azure subscription
     - **Resource Group**: Create new or select existing
     - **Name**: Choose a name (e.g., `observability-sample-app-insights`)
     - **Region**: Select your preferred region
     - **Resource Mode**: Choose "Workspace-based"
   - Click "Review + create" then "Create"

3. **Get Connection String**:
   - Once created, go to your Application Insights resource
   - In the Overview section, copy the **Connection String**
   - It will look like: `InstrumentationKey=12345678-1234-1234-1234-123456789012;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/`

## Step 2: Configure the Application

1. **Set Connection String**:
   ```bash
   # Option 1: Update the .env file
   echo "APPLICATIONINSIGHTS_CONNECTION_STRING=YOUR_CONNECTION_STRING_HERE" > .env
   
   # Option 2: Set environment variable directly
   export APPLICATIONINSIGHTS_CONNECTION_STRING="YOUR_CONNECTION_STRING_HERE"
   ```

2. **Rebuild and Start Services**:
   ```bash
   docker compose down
   docker compose up -d --build
   ```

## Step 3: Test the New Endpoint

1. **Make requests to the new endpoint**:
   ```bash
   # Test successful requests
   curl "http://localhost:3000/azure/call?partner=test-partner"
   
   # Make multiple requests to generate metrics
   for i in {1..10}; do
     curl "http://localhost:3000/azure/call?partner=partner-$i"
     sleep 1
   done
   ```

## Step 4: View Metrics and Logs in Azure Portal

### Viewing Custom Metrics

1. Go to your Application Insights resource in Azure Portal
2. Navigate to **Metrics** in the left sidebar
3. Create a new chart:
   - **Metric Namespace**: Select "Custom"
   - **Metric**: Choose from:
     - `azure_requests_per_second` - Request counter
     - `azure_request_latency_ms` - Response time latency
     - `azure_request_errors` - Error count
4. Add filters by clicking "Add filter":
   - **Property**: `partner` (to filter by partner)
   - **Property**: `endpoint` (to filter by endpoint)

### Viewing Logs

1. Go to your Application Insights resource in Azure Portal
2. Navigate to **Logs** in the left sidebar
3. Use KQL (Kusto Query Language) to query logs:

   ```kql
   // View all traces from the azure/call endpoint
   traces
   | where customDimensions.endpoint == "/azure/call"
   | order by timestamp desc
   
   // View traces by partner
   traces
   | where customDimensions.partner == "test-partner"
   | order by timestamp desc
   
   // View error logs
   exceptions
   | where customDimensions.endpoint == "/azure/call"
   | order by timestamp desc
   ```

### Creating Dashboards

1. Navigate to **Dashboards** in Application Insights
2. Click "New dashboard"
3. Add tiles for:
   - **Requests per second**: Pin your custom metric chart
   - **Average latency**: Create a chart for `azure_request_latency_ms`
   - **Error rate**: Create a chart for `azure_request_errors`
   - **Recent logs**: Pin a log query

### Setting Up Alerts

1. Go to **Alerts** in your Application Insights resource
2. Click "Create alert rule"
3. Configure conditions based on your custom metrics:
   - **Signal**: Custom metric (e.g., `azure_request_errors`)
   - **Condition**: Greater than threshold
   - **Threshold**: Set your desired error threshold
4. Configure action groups for notifications (email, SMS, etc.)

## Metrics Being Tracked

The `/azure/call` endpoint tracks the following metrics:

1. **azure_requests_per_second**: Counter of requests per second
2. **azure_request_latency_ms**: Response time in milliseconds  
3. **azure_request_errors**: Count of errors

## Logs Being Sent

The endpoint sends the following log types:

1. **Information logs**: For successful requests
2. **Exception logs**: For errors and failures
3. **Custom properties**: Partner, endpoint, timestamp information

## Troubleshooting

### Connection Issues
- Verify your connection string is correct
- Check that the Application Insights resource is active
- Ensure network connectivity to Azure

### No Data Appearing
- Wait 2-5 minutes for data to appear in Azure Portal
- Check container logs: `docker logs node-service`
- Verify requests are being made to `/azure/call`

### Invalid Connection String
- Ensure the connection string includes both InstrumentationKey and IngestionEndpoint
- Check for any extra spaces or characters
