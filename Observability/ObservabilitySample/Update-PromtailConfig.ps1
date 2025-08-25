# PowerShell script to update Promtail configuration with current node-service container ID

Write-Host "Finding node-service container..." -ForegroundColor Cyan

# Get the container ID for node-service
$containerId = docker ps --filter "name=node-service" --format "{{.ID}}"

if ([string]::IsNullOrEmpty($containerId)) {
    Write-Error "Error: node-service container not found. Make sure containers are running."
    Write-Host "Try: docker compose up -d" -ForegroundColor Yellow
    exit 1
}

Write-Host "Found node-service container ID: $containerId" -ForegroundColor Green

# Read the current config
$configPath = "./promtail/promtail-config.yml"
$configContent = Get-Content $configPath -Raw

# Replace both placeholder and any existing container ID pattern
$updatedConfig = $configContent -replace "CONTAINER_ID_PLACEHOLDER", $containerId
$updatedConfig = $updatedConfig -replace "/var/lib/docker/containers/[a-f0-9]{12}\*", "/var/lib/docker/containers/$containerId*"

# Write back to file
$updatedConfig | Out-File -FilePath $configPath -Encoding UTF8 -NoNewline

Write-Host "Updated promtail configuration with container ID: $containerId" -ForegroundColor Green
Write-Host "Restarting Promtail container..." -ForegroundColor Cyan

docker restart promtail | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "Done! Promtail has been updated and restarted." -ForegroundColor Green
    Write-Host ""
    Write-Host "Test with: curl `"http://localhost:3000/call?partner=test123`"" -ForegroundColor Cyan
} else {
    Write-Host "Failed to restart Promtail container" -ForegroundColor Red
    exit 1
}
