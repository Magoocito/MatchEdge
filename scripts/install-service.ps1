# MatchEdge - Windows Service Installation Script
# Run as Administrator

param(
    [string]$ServiceName = "MatchEdge",
    [string]$DisplayName = "MatchEdge Betting Platform",
    [string]$Description = "MatchEdge automated betting platform with FootyMetrics integration",
    [string]$PublishPath = "C:\Services\MatchEdge"
)

Write-Host "=== MatchEdge Service Installer ===" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    exit 1
}

# Stop and remove existing service if present
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "Stopping existing service..." -ForegroundColor Yellow
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 3
    
    Write-Host "Removing existing service..." -ForegroundColor Yellow
    & sc.exe delete $ServiceName
    Start-Sleep -Seconds 2
}

# Publish the application
Write-Host "Publishing MatchEdge..." -ForegroundColor Green
$projectPath = Join-Path $PSScriptRoot "src\MatchEdge.Api\MatchEdge.Api.csproj"

dotnet publish $projectPath `
    --configuration Release `
    --output $PublishPath `
    --self-contained false `
    --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Published to: $PublishPath" -ForegroundColor Green

# Copy chrome profile if it exists
$chromeProfileSource = Join-Path $PSScriptRoot "chrome-profile-footymetrics"
$chromeProfileDest = Join-Path $PublishPath "chrome-profile-footymetrics"

if (Test-Path $chromeProfileSource) {
    Write-Host "Copying Chrome profile..." -ForegroundColor Yellow
    if (-not (Test-Path $chromeProfileDest)) {
        Copy-Item -Path $chromeProfileSource -Destination $chromeProfileDest -Recurse
    }
}

# Create the Windows Service
Write-Host "Creating Windows Service: $ServiceName..." -ForegroundColor Green
$exePath = Join-Path $PublishPath "MatchEdge.Api.exe"

& sc.exe create $ServiceName `
    binPath= "`"$exePath`"" `
    DisplayName= "$DisplayName" `
    start= auto `
    obj= "LocalSystem"

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to create service!" -ForegroundColor Red
    exit 1
}

# Configure service recovery (restart on failure)
& sc.exe failure $ServiceName reset= 86400 actions= restart/5000/restart/10000/restart/30000

# Set description
& sc.exe description $ServiceName "$Description"

Write-Host ""
Write-Host "=== Service Installed Successfully ===" -ForegroundColor Green
Write-Host "Service Name: $ServiceName" -ForegroundColor Cyan
Write-Host "Display Name: $DisplayName" -ForegroundColor Cyan
Write-Host "Executable: $exePath" -ForegroundColor Cyan
Write-Host "API URL: http://localhost:5272" -ForegroundColor Cyan
Write-Host "Swagger: http://localhost:5272/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "To start the service:" -ForegroundColor Yellow
Write-Host "  Start-Service -Name $ServiceName"
Write-Host ""
Write-Host "To stop the service:" -ForegroundColor Yellow
Write-Host "  Stop-Service -Name $ServiceName"
Write-Host ""
Write-Host "To view logs:" -ForegroundColor Yellow
Write-Host "  Get-EventLog -LogName Application -Source '$ServiceName' -Newest 10"
