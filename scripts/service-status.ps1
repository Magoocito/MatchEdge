# MatchEdge - Service Status Script

param(
    [string]$ServiceName = "MatchEdge"
)

Write-Host "=== MatchEdge Service Status ===" -ForegroundColor Cyan
Write-Host ""

# Get service status
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($service) {
    Write-Host "Service Status: $($service.Status)" -ForegroundColor $(if ($service.Status -eq "Running") { "Green" } else { "Yellow" })
    Write-Host "Start Type: $($service.StartType)"
    Write-Host "Display Name: $($service.DisplayName)"
    Write-Host "Description: $($service.Description)"
} else {
    Write-Host "Service not found." -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test API endpoint
Write-Host "Testing API endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5272/api/FootyMetricsTest/pipeline/status" -Method GET -TimeoutSec 5 -UseBasicParsing
    $status = $response.Content | ConvertFrom-Json
    
    Write-Host "API Status: OK" -ForegroundColor Green
    Write-Host "Pipeline Running: $($status.isRunning)"
    Write-Host "Last Run: $($status.lastRun)"
    Write-Host "Runs Today: $($status.totalRunsToday)"
    Write-Host "Last Result: $($status.lastResult)"
} catch {
    Write-Host "API Status: Not responding" -ForegroundColor Red
}

Write-Host ""

# Show recent logs
Write-Host "Recent Application Logs:" -ForegroundColor Yellow
try {
    $logs = Get-EventLog -LogName Application -Source $ServiceName -Newest 5 -ErrorAction SilentlyContinue
    if ($logs) {
        foreach ($log in $logs) {
            Write-Host "  $($log.TimeGenerated) - $($log.Message)" -ForegroundColor Gray
        }
    } else {
        Write-Host "  No logs found." -ForegroundColor Gray
    }
} catch {
    Write-Host "  Could not read event logs." -ForegroundColor Gray
}
