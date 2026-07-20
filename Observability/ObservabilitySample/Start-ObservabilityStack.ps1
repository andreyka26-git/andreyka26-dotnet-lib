# PowerShell script to rebuild the observability stack with proper log configuration

Write-Host " Stopping containers..." -ForegroundColor Yellow
docker compose down

Write-Host " Starting containers..." -ForegroundColor Green
docker compose up --build -d

Write-Host " Waiting for containers to start..." -ForegroundColor Cyan
Start-Sleep 10

Write-Host " Updating Promtail configuration with current container ID..." -ForegroundColor Cyan
.\Update-PromtailConfig.ps1

Write-Host ""
Write-Host " Observability stack is ready!" -ForegroundColor Green
Write-Host " Grafana: http://localhost:3001 (admin/admin)" -ForegroundColor Blue
Write-Host " Prometheus: http://localhost:9090" -ForegroundColor Blue
Write-Host " Loki: http://localhost:3100" -ForegroundColor Blue
Write-Host " Node Service: http://localhost:3000" -ForegroundColor Blue
Write-Host ""
Write-Host " Test the setup:" -ForegroundColor Yellow
Write-Host '   curl "http://localhost:3000/call?partner=test123"' -ForegroundColor White
Write-Host ""
