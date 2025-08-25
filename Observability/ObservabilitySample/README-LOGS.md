# Observability Stack with Dynamic Log Configuration

This setup provides a complete observability stack with Prometheus (metrics), Loki (logs), and Grafana (visualization) that automatically configures itself to capture only logs from your Node.js service.

## Quick Start

### Windows (PowerShell)

**Option 1: Start Everything at Once**
```powershell
.\Start-ObservabilityStack.ps1
```

**Option 2: Manual Steps**
```powershell
# Start the stack
docker compose up --build -d

# Update Promtail configuration for current container
.\Update-PromtailConfig.ps1
```

### Mac/Linux (Bash)

**Option 1: Start Everything at Once**
```bash
./start-observability-stack.sh
```

**Option 2: Manual Steps**
```bash
# Start the stack
docker compose up --build -d

# Update Promtail configuration for current container
./update-promtail-config.sh
```

## The Container ID Problem & Solution

### ❌ The Problem
Docker containers get new random IDs every time they're recreated:
- `359423f6f6fc` → becomes → `a1b2c3d4e5f6` after restart
- Hardcoded container IDs in Promtail config will break after `docker compose down/up`

### ✅ The Solution
**Container ID Override Scripts:**
1. **Discover** the current `node-service` container ID dynamically
2. **Replace** placeholder in Promtail configuration with the current ID  
3. **Restart** Promtail to apply changes
4. **Ensure** only your application logs are captured

**Why not Docker service discovery?**
- The ideal solution would be using `docker_sd_configs` with `filters: [name: label, values: ["logging=promtail"]]`
- However, Docker socket access (`/var/run/docker.sock`) doesn't work reliably on Windows Docker Desktop
- Container ID override is a robust cross-platform workaround

## Files Created

### Windows
- **`Update-PromtailConfig.ps1`** - Updates Promtail config with current container ID
- **`Start-ObservabilityStack.ps1`** - Complete stack startup script

### Mac/Linux  
- **`update-promtail-config.sh`** - Updates Promtail config with current container ID
- **`start-observability-stack.sh`** - Complete stack startup script

## When to Run the Update Script

Run the update script whenever you:
- Restart containers (`docker compose down && docker compose up`)
- Rebuild containers (`docker compose up --build`)
- Notice logs not appearing in Grafana

**Windows:**
```powershell
.\Update-PromtailConfig.ps1
```

**Mac/Linux:**
```bash
./update-promtail-config.sh
```

## Access Points

- **Grafana Dashboard**: http://localhost:3001 (admin/admin)
- **Prometheus**: http://localhost:9090  
- **Loki**: http://localhost:3100
- **Node Service**: http://localhost:3000

## Testing

Generate test logs:
```bash
curl "http://localhost:3000/call?partner=test123"
curl "http://localhost:3000/call?partner=errorTest&error=true"
```

Check if logs appear in:
1. Grafana dashboard → "Application Logs" panel
2. Direct Loki query: http://localhost:3100

## Log Filtering

The configuration ensures you see **ONLY**:
- ✅ Your Node.js application logs (`console.log` statements)
- ✅ Logs from the `node-service` container
- ❌ NO system logs from Loki, Promtail, Grafana, etc.
- ❌ NO Docker daemon logs

## Troubleshooting

**No logs in Grafana?**
1. Run the appropriate update script for your platform
2. Generate test logs with `curl`
3. Check container is running: `docker ps`

**Container ID changed?**
- This happens after every restart - just run the update script again!

**Script execution error on Windows?**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Script permission error on Mac/Linux?**
```bash
chmod +x update-promtail-config.sh
chmod +x start-observability-stack.sh
```
