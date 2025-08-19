#!/bin/bash

# Observability Demo System - Demo Script
# This script demonstrates the observability features of the system

echo "🚀 Starting Observability Demo System..."
echo "========================================="

# Function to check if a service is responding
check_service() {
    local url=$1
    local service_name=$2
    echo "Checking $service_name..."
    
    for i in {1..30}; do
        if curl -s "$url" > /dev/null 2>&1; then
            echo "✅ $service_name is ready!"
            return 0
        fi
        echo "⏳ Waiting for $service_name... (attempt $i/30)"
        sleep 2
    done
    
    echo "❌ $service_name failed to start"
    return 1
}

# Start all services
echo "1️⃣ Starting all services with Docker Compose..."
docker-compose up -d

echo ""
echo "2️⃣ Waiting for services to start..."
sleep 10

# Check service health
echo ""
echo "3️⃣ Checking service health..."
check_service "http://localhost:8080/api/orders" "Service1 (Orders API)"
check_service "http://localhost:8081/api/events/health" "Service2 (Events API)"
check_service "http://localhost:3001" "Client1 (React App)"
check_service "http://localhost:9090" "Prometheus"
check_service "http://localhost:3000" "Grafana"
check_service "http://localhost:3100/ready" "Loki"

echo ""
echo "4️⃣ Creating sample data..."

# Create some sample orders
echo "Creating sample orders..."
for order_type in "Electronics" "Books" "Clothing" "Sports" "Home"; do
    echo "Creating order: $order_type"
    curl -X POST "http://localhost:8080/api/orders" \
         -H "Content-Type: application/json" \
         -d "{\"orderType\": \"$order_type\"}" \
         -s > /dev/null
    sleep 1
done

echo ""
echo "5️⃣ Generating some load..."
# Generate some load to create metrics
for i in {1..20}; do
    curl -s "http://localhost:8080/api/orders" > /dev/null &
    sleep 0.5
done

wait

echo ""
echo "🎉 Demo system is ready!"
echo "========================"
echo ""
echo "📱 Access Points:"
echo "   Demo Application:    http://localhost:3001"
echo "   Grafana Dashboards:  http://localhost:3000 (admin/admin)"
echo "   Prometheus:          http://localhost:9090"
echo "   Alertmanager:        http://localhost:9093"
echo ""
echo "📊 Metrics Endpoints:"
echo "   Service1 (.NET):     http://localhost:8080/metrics"
echo "   Service2 (Node.js):  http://localhost:8081/metrics"
echo ""
echo "🔍 Demo Actions:"
echo "   1. Go to http://localhost:3001 and create some orders"
echo "   2. View metrics in Grafana at http://localhost:3000"
echo "   3. Check Prometheus targets at http://localhost:9090/targets"
echo "   4. View logs in Grafana > Explore > Loki"
echo ""
echo "🧪 Test Scenarios:"
echo "   • Stop a service: docker-compose stop service2"
echo "   • Generate errors and watch alerts"
echo "   • Monitor database latency"
echo "   • Trace requests across services"
echo ""
echo "🛑 To stop the demo:"
echo "   docker-compose down -v"
