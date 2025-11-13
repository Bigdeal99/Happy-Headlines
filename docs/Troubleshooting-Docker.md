# Docker Troubleshooting Guide

## Issue: "unexpected EOF" or "failed commit" or Docker Digest Mismatch

### Symptoms:
- `unexpected EOF` during image download/extraction
- `failed commit on ref "layer-sha256:..."` with digest mismatch
- `500 Internal Server Error` from Docker API
- Docker Desktop becomes unresponsive

### Root Causes:
1. **Corrupted Docker build cache** - Most common
2. **Network interruption** during image download
3. **Docker Desktop instability** - Needs restart
4. **Disk space issues** - Not enough space for images
5. **Concurrent builds** - Multiple builds interfering

### Solution Steps:

### Quick Fix

1. **Start Docker Desktop**
   - Open Docker Desktop application
   - Wait for it to fully start (whale icon should be steady, not animated)
   - Check system tray for Docker icon

2. **Verify Docker is Running**
   ```powershell
   docker ps
   ```
   Should return container list (or empty if no containers running)

3. **Restart All Services**
   ```powershell
   cd C:\Users\marce\Documents\GitHub\Happy-Headlines
   docker compose down
   docker compose up -d --build
   ```

## Common Issues and Solutions

### Issue 1: Docker Desktop Not Running

**Symptoms:**
- Error: `The system cannot find the file specified`
- Error: `error during connect: Get "http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine"`

**Solution:**
1. Open Docker Desktop
2. Wait for it to start completely
3. Check if Docker Desktop shows "Docker Desktop is running"

### Issue 2: Container Crashed

**Symptoms:**
- Container exits immediately
- Status shows "Exited (1)" or similar

**Solution:**
```powershell
# Check logs for the crashed container
docker compose logs subscriber-service
docker compose logs newsletter-service

# Restart specific service
docker compose restart subscriber-service

# Or rebuild and restart
docker compose up -d --build subscriber-service
```

### Issue 3: Port Conflicts

**Symptoms:**
- Error: `port is already allocated`
- Service won't start

**Solution:**
```powershell
# Find what's using the port
netstat -ano | findstr :8088

# Stop conflicting process or change port in docker-compose.yml
```

### Issue 4: Out of Memory/Disk Space

**Symptoms:**
- Containers keep restarting
- Docker becomes slow
- "No space left on device" errors

**Solution:**
```powershell
# Clean up unused resources
docker system prune -a

# Check disk space
docker system df

# Remove unused volumes
docker volume prune
```

### Issue 5: Database Connection Issues

**Symptoms:**
- Services can't connect to database
- "Connection refused" errors

**Solution:**
```powershell
# Wait for database to be ready
docker compose logs subscriber-db

# Restart service after database is ready
docker compose restart subscriber-service
```

## Step-by-Step Recovery

### Complete Restart Procedure

```powershell
# 1. Stop all containers
docker compose down

# 2. Clean up (optional, if having issues)
docker system prune -f

# 3. Start Docker Desktop (if not running)
# Open Docker Desktop application

# 4. Wait for Docker to be ready
docker ps

# 5. Start all services
cd C:\Users\marce\Documents\GitHub\Happy-Headlines
docker compose up -d --build

# 6. Check status
docker compose ps

# 7. Check logs if issues persist
docker compose logs -f
```

## Checking Service Health

### Check All Services Status
```powershell
docker compose ps
```

Expected output should show all services as "Up" or "running"

### Check Specific Service Logs
```powershell
# SubscriberService
docker compose logs subscriber-service

# NewsletterService
docker compose logs newsletter-service

# Database
docker compose logs subscriber-db

# All services
docker compose logs
```

### Check Service Health Endpoints
```powershell
# SubscriberService (should return subscribers or 503 if disabled)
Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers"

# NewsletterService
Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/newsletter/send"
```

## Docker Desktop Settings

If Docker Desktop keeps stopping:

1. **Check Resources:**
   - Docker Desktop → Settings → Resources
   - Ensure enough memory allocated (at least 4GB)
   - Ensure enough disk space

2. **Check WSL 2 (if using):**
   ```powershell
   wsl --list --verbose
   wsl --update
   ```

3. **Restart Docker Desktop:**
   - Right-click Docker icon in system tray
   - Select "Restart Docker Desktop"

## Emergency Recovery

If nothing works:

```powershell
# 1. Stop everything
docker compose down -v

# 2. Restart Docker Desktop completely

# 3. Clean everything
docker system prune -a --volumes

# 4. Rebuild from scratch
docker compose up -d --build
```

## Getting Help

If issues persist:

1. Check Docker Desktop logs:
   - Docker Desktop → Troubleshoot → View logs

2. Check Windows Event Viewer:
   - Look for Docker-related errors

3. Check service-specific logs:
   ```powershell
   docker compose logs subscriber-service > subscriber-logs.txt
   docker compose logs newsletter-service > newsletter-logs.txt
   ```

---

**Quick Reference:**
- Start Docker Desktop: Open the application
- Check status: `docker compose ps`
- View logs: `docker compose logs [service-name]`
- Restart service: `docker compose restart [service-name]`
- Full restart: `docker compose down && docker compose up -d --build`

