# MatchEdge - Windows Service Uninstall Script
# Run as Administrator

param(
    [string]$ServiceName = "MatchEdge",
    [string]$PublishPath = "C:\Services\MatchEdge"
)

Write-Host "=== MatchEdge Service Uninstaller ===" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    exit 1
}

# Stop the service
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($service) {
    if ($service.Status -eq "Running") {
        Write-Host "Stopping service..." -ForegroundColor Yellow
        Stop-Service -Name $ServiceName -Force
        Start-Sleep -Seconds 3
    }
    
    Write-Host "Removing service..." -ForegroundColor Yellow
    & sc.exe delete $ServiceName
    Start-Sleep -Seconds 2
    
    Write-Host "Service removed." -ForegroundColor Green
} else {
    Write-Host "Service not found." -ForegroundColor Yellow
}

# Remove published files (optional)
$removeFiles = Read-Host "Do you want to remove published files at $PublishPath? (y/N)"
if ($removeFiles -eq "y" -or $removeFiles -eq "Y") {
    if (Test-Path $PublishPath) {
        Write-Host "Removing files..." -ForegroundColor Yellow
        Remove-Item -Path $PublishPath -Recurse -Force
        Write-Host "Files removed." -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=== Uninstall Complete ===" -ForegroundColor Green
