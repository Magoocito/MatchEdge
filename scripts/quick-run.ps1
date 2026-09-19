# MatchEdge - Quick Publish & Run Script (for testing)

param(
    [string]$PublishPath = "C:\Services\MatchEdge",
    [switch]$SkipPublish
)

Write-Host "=== MatchEdge Quick Run ===" -ForegroundColor Cyan
Write-Host ""

# Publish
if (-not $SkipPublish) {
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
}

# Copy chrome profile
$chromeProfileSource = Join-Path $PSScriptRoot "chrome-profile-footymetrics"
$chromeProfileDest = Join-Path $PublishPath "chrome-profile-footymetrics"

if (Test-Path $chromeProfileSource) {
    if (-not (Test-Path $chromeProfileDest)) {
        Write-Host "Copying Chrome profile..." -ForegroundColor Yellow
        Copy-Item -Path $chromeProfileSource -Destination $chromeProfileDest -Recurse
    }
}

# Run
Write-Host "Starting MatchEdge..." -ForegroundColor Green
$exePath = Join-Path $PublishPath "MatchEdge.Api.exe"

Write-Host "API URL: http://localhost:5272" -ForegroundColor Cyan
Write-Host "Swagger: http://localhost:5272/swagger" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

& $exePath
