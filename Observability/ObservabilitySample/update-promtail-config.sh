#!/bin/bash

# Bash script to update Promtail configuration with current node-service container ID

echo "🔍 Finding node-service container..."

# Get the container ID for node-service
CONTAINER_ID=$(docker ps --filter "name=node-service" --format "{{.ID}}")

if [ -z "$CONTAINER_ID" ]; then
    echo "❌ Error: node-service container not found. Make sure containers are running."
    echo "💡 Try: docker compose up -d"
    exit 1
fi

echo "✅ Found node-service container ID: $CONTAINER_ID"

# Replace the placeholder with actual container ID in the config file
CONFIG_FILE="./promtail/promtail-config.yml"

if [ ! -f "$CONFIG_FILE" ]; then
    echo "❌ Error: $CONFIG_FILE not found"
    exit 1
fi

# Use sed to replace the placeholder
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS requires different sed syntax
    sed -i '' "s/CONTAINER_ID_PLACEHOLDER/$CONTAINER_ID/g" "$CONFIG_FILE"
else
    # Linux sed syntax
    sed -i "s/CONTAINER_ID_PLACEHOLDER/$CONTAINER_ID/g" "$CONFIG_FILE"
fi

echo "📝 Updated promtail configuration with container ID: $CONTAINER_ID"
echo "🔄 Restarting Promtail container..."

if docker restart promtail > /dev/null 2>&1; then
    echo "✅ Done! Promtail has been updated and restarted."
    echo ""
    echo "🧪 Test with: curl \"http://localhost:3000/call?partner=test123\""
else
    echo "❌ Failed to restart Promtail container"
    exit 1
fi
