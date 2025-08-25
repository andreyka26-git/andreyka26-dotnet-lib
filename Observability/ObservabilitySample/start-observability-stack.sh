#!/bin/bash

# Bash script to rebuild the observability stack with proper log configuration

echo "🛑 Stopping containers..."
docker compose down

echo "🚀 Starting containers..." 
docker compose up --build -d

echo "⏳ Waiting for containers to start..."
sleep 10

echo "🔧 Updating Promtail configuration with current container ID..."
./update-promtail-config.sh

echo ""
echo "✅ Observability stack is ready!"
echo "🔗 Grafana: http://localhost:3001 (admin/admin)"
echo "🔗 Prometheus: http://localhost:9090"  
echo "🔗 Loki: http://localhost:3100"
echo "🔗 Node Service: http://localhost:3000"
echo ""
echo "💡 Test the setup:"
echo "   curl \"http://localhost:3000/call?partner=test123\""
echo ""
