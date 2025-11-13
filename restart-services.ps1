Write-Host "=== Restarting Happy Headlines Services ===" -ForegroundColor Green

Write-Host "`n1. Stopping all services..." -ForegroundColor Yellow
docker compose down

Write-Host "`n2. Cleaning up Docker system (optional)..." -ForegroundColor Yellow
Write-Host "   (Skipping to save time - uncomment next line if needed)" -ForegroundColor Gray
# docker system prune -f

Write-Host "`n3. Starting services (this may take a few minutes)..." -ForegroundColor Yellow
docker compose up -d --build

Write-Host "`n4. Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n5. Checking service status..." -ForegroundColor Yellow
docker compose ps

Write-Host "`n=== Services Restarted ===" -ForegroundColor Green
Write-Host "`nCheck logs if services are not running:" -ForegroundColor Cyan
Write-Host "  docker compose logs [service-name]" -ForegroundColor Gray
Write-Host "`nTest SubscriberService:" -ForegroundColor Cyan
Write-Host "  .\test-subscriber-service.ps1" -ForegroundColor Gray

