@echo off
REM Observability Demo System - Demo Script for Windows
REM This script demonstrates the observability features of the system

echo 🚀 Starting Observability Demo System...
echo =========================================

echo 1️⃣ Starting all services with Docker Compose...
docker-compose up -d

echo.
echo 2️⃣ Waiting for services to start...
timeout /t 20 /nobreak >nul

echo.
echo 3️⃣ Checking service health...
echo Checking services...
timeout /t 10 /nobreak >nul

echo.
echo 4️⃣ Creating sample data...
echo Creating sample orders...

curl -X POST "http://localhost:8080/api/orders" -H "Content-Type: application/json" -d "{\"orderType\": \"Electronics\"}" >nul 2>&1
timeout /t 1 /nobreak >nul
curl -X POST "http://localhost:8080/api/orders" -H "Content-Type: application/json" -d "{\"orderType\": \"Books\"}" >nul 2>&1
timeout /t 1 /nobreak >nul
curl -X POST "http://localhost:8080/api/orders" -H "Content-Type: application/json" -d "{\"orderType\": \"Clothing\"}" >nul 2>&1
timeout /t 1 /nobreak >nul
curl -X POST "http://localhost:8080/api/orders" -H "Content-Type: application/json" -d "{\"orderType\": \"Sports\"}" >nul 2>&1
timeout /t 1 /nobreak >nul
curl -X POST "http://localhost:8080/api/orders" -H "Content-Type: application/json" -d "{\"orderType\": \"Home\"}" >nul 2>&1

echo.
echo 5️⃣ Generating some load...
for /l %%i in (1,1,10) do (
    start /b curl -s "http://localhost:8080/api/orders" >nul 2>&1
    timeout /t 1 /nobreak >nul
)

echo.
echo 🎉 Demo system is ready!
echo ========================
echo.
echo 📱 Access Points:
echo    Demo Application:    http://localhost:3001
echo    Grafana Dashboards:  http://localhost:3000 (admin/admin)
echo    Prometheus:          http://localhost:9090
echo    Alertmanager:        http://localhost:9093
echo.
echo 📊 Metrics Endpoints:
echo    Service1 (.NET):     http://localhost:8080/metrics
echo    Service2 (Node.js):  http://localhost:8081/metrics
echo.
echo 🔍 Demo Actions:
echo    1. Go to http://localhost:3001 and create some orders
echo    2. View metrics in Grafana at http://localhost:3000
echo    3. Check Prometheus targets at http://localhost:9090/targets
echo    4. View logs in Grafana ^> Explore ^> Loki
echo.
echo 🧪 Test Scenarios:
echo    • Stop a service: docker-compose stop service2
echo    • Generate errors and watch alerts
echo    • Monitor database latency
echo    • Trace requests across services
echo.
echo 🛑 To stop the demo:
echo    docker-compose down -v

pause
