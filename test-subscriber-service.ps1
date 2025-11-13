Write-Host "=== Testing SubscriberService ===" -ForegroundColor Green

# Test 1: Subscribe
Write-Host "`n1. Testing Subscribe..." -ForegroundColor Yellow
$body = @{
    email = "test@example.com"
    name = "Test User"
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body
    Write-Host "✓ Subscribe successful: ID=$($result.id), Email=$($result.email)" -ForegroundColor Green
} catch {
    Write-Host "✗ Subscribe failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "  Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    }
}

# Test 2: Get Subscribers
Write-Host "`n2. Testing Get Subscribers..." -ForegroundColor Yellow
try {
    $subscribers = Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers?activeOnly=true"
    Write-Host "✓ Found $($subscribers.Count) active subscribers" -ForegroundColor Green
    foreach ($sub in $subscribers) {
        Write-Host "  - $($sub.email) ($($sub.name))" -ForegroundColor Cyan
    }
} catch {
    Write-Host "✗ Get subscribers failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Feature Flag
Write-Host "`n3. Testing Feature Flag..." -ForegroundColor Yellow
try {
    $flag = Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers/feature-flag"
    Write-Host "✓ Feature flag status: $($flag.enabled)" -ForegroundColor Green
    
    # Test disabling
    Write-Host "  Disabling feature flag..." -ForegroundColor Yellow
    $disableBody = @{ enabled = $false } | ConvertTo-Json
    Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers/feature-flag" -ContentType "application/json" -Body $disableBody | Out-Null
    
    # Try to subscribe (should fail)
    Write-Host "  Attempting subscribe (should fail with 503)..." -ForegroundColor Yellow
    try {
        $testBody = @{ email = "disabled-test@example.com"; name = "Disabled Test" } | ConvertTo-Json
        Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $testBody | Out-Null
        Write-Host "  ✗ Should have failed but didn't!" -ForegroundColor Red
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -eq 503) {
            Write-Host "  ✓ Correctly returned 503 (Service Unavailable)" -ForegroundColor Green
        } else {
            Write-Host "  ✗ Wrong status code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        }
    }
    
    # Re-enable
    Write-Host "  Re-enabling feature flag..." -ForegroundColor Yellow
    $enableBody = @{ enabled = $true } | ConvertTo-Json
    Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers/feature-flag" -ContentType "application/json" -Body $enableBody | Out-Null
    Write-Host "  ✓ Feature flag re-enabled" -ForegroundColor Green
} catch {
    Write-Host "✗ Feature flag test failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Newsletter Integration
Write-Host "`n4. Testing NewsletterService Integration..." -ForegroundColor Yellow
try {
    $newsletter = Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/newsletter/send"
    Write-Host "✓ Newsletter sent successfully" -ForegroundColor Green
    Write-Host "  Subscribers: $($newsletter.subscribersCount)" -ForegroundColor Cyan
    Write-Host "  Articles retrieved: $($newsletter.preview.Length) characters" -ForegroundColor Cyan
    if ($newsletter.subscribersCount -gt 0) {
        Write-Host "  Subscriber list:" -ForegroundColor Cyan
        foreach ($sub in $newsletter.subscribers) {
            Write-Host "    - $($sub.email) ($($sub.name))" -ForegroundColor Cyan
        }
    }
} catch {
    Write-Host "✗ Newsletter send failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Fault Isolation (if SubscriberService is down)
Write-Host "`n5. Testing Fault Isolation..." -ForegroundColor Yellow
Write-Host "  (This test requires SubscriberService to be stopped)" -ForegroundColor Gray
Write-Host "  Run: docker compose stop subscriber-service" -ForegroundColor Gray
Write-Host "  Then run this script again to test fault isolation" -ForegroundColor Gray

Write-Host "`n=== Testing Complete ===" -ForegroundColor Green
Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "1. Check RabbitMQ UI: http://localhost:15673 (guest/guest)" -ForegroundColor Cyan
Write-Host "2. Check Jaeger traces: http://localhost:16686" -ForegroundColor Cyan
Write-Host "3. Test unsubscribe: DELETE http://localhost:8088/api/subscribers/test@example.com" -ForegroundColor Cyan

