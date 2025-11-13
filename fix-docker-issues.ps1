Write-Host "=== Fixing Docker Issues ===" -ForegroundColor Green

Write-Host "`nStep 1: Stopping all containers..." -ForegroundColor Yellow
docker compose down 2>$null

Write-Host "`nStep 2: Cleaning Docker build cache..." -ForegroundColor Yellow
docker builder prune -af

Write-Host "`nStep 3: Cleaning unused Docker resources..." -ForegroundColor Yellow
docker system prune -f

Write-Host "`nStep 4: Checking Docker Desktop status..." -ForegroundColor Yellow
$dockerRunning = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerRunning) {
    Write-Host "  ⚠ Docker Desktop is not running!" -ForegroundColor Red
    Write-Host "  Please start Docker Desktop and wait for it to be ready, then run this script again." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "  ✓ Docker Desktop is running" -ForegroundColor Green
}

Write-Host "`nStep 5: Testing Docker connection..." -ForegroundColor Yellow
try {
    docker ps | Out-Null
    Write-Host "  ✓ Docker is responding" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker is not responding. Please restart Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "`nStep 6: Rebuilding services (this may take several minutes)..." -ForegroundColor Yellow
Write-Host "  Building SubscriberService first..." -ForegroundColor Cyan
docker compose build --no-cache subscriber-service

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ SubscriberService built successfully" -ForegroundColor Green
    Write-Host "`nStep 7: Starting all services..." -ForegroundColor Yellow
    docker compose up -d
    
    Write-Host "`nStep 8: Waiting for services to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 20
    
    Write-Host "`nStep 9: Checking service status..." -ForegroundColor Yellow
    docker compose ps
    
    Write-Host "`n=== Recovery Complete ===" -ForegroundColor Green
    Write-Host "`nIf services are still having issues:" -ForegroundColor Yellow
    Write-Host "  1. Check logs: docker compose logs [service-name]" -ForegroundColor Cyan
    Write-Host "  2. Restart Docker Desktop completely" -ForegroundColor Cyan
    Write-Host "  3. Check disk space: docker system df" -ForegroundColor Cyan
} else {
    Write-Host "  ✗ Build failed. Try:" -ForegroundColor Red
    Write-Host "    1. Restart Docker Desktop" -ForegroundColor Yellow
    Write-Host "    2. Check disk space" -ForegroundColor Yellow
    Write-Host "    3. Run: docker system prune -a --volumes" -ForegroundColor Yellow
}

