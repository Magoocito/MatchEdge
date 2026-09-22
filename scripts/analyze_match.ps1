# SCRIPT DE ANÁLISIS AUTOMATIZADO DE PARTIDOS
# Uso: .\analyze_match.ps1 -FixtureUrl "https://www.footymetrics.com/fixtures/..."

param(
    [Parameter(Mandatory=$true)]
    [string]$FixtureUrl,
    
    [Parameter(Mandatory=$false)]
    [string]$ApiBase = "http://localhost:5272/api/footymetricsTest"
)

# Función para extraer datos del Overview
function Get-OverviewData {
    param([string]$Url)
    
    Write-Host "=== EXTRAYENDO OVERVIEW ===" -ForegroundColor Cyan
    
    $overviewUrl = "$Url"
    $result = Invoke-RestMethod -Uri "$ApiBase/page-content?url=$overviewUrl" -TimeoutSec 60
    
    $lines = $result.contentPreview -split "`n"
    $data = @{
        Alineaciones = @()
        RecentForm = @()
        H2H = @()
        Estadisticas = @()
    }
    
    # Extraer alineaciones
    $inLineups = $false
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "lineup|Starting|Formation") {
            $inLineups = $true
        }
        if ($inLineups -and $lines[$i] -match "^\d+\s+\w") {
            $data.Alineaciones += $lines[$i].Trim()
        }
    }
    
    # Extraer Recent Form
    $inForm = $false
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "Recent form") {
            $inForm = $true
            $start = $i
        }
        if ($inForm -and $i -lt $start + 15) {
            if ($lines[$i] -match "\d+ - \d+" -or $lines[$i] -match "Win|Loss|Draw") {
                $data.RecentForm += $lines[$i].Trim()
            }
        }
    }
    
    # Extraer H2H
    $inH2H = $false
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "H2H|Head to head") {
            $inH2H = $true
        }
        if ($inH2H -and $lines[$i] -match "meetings|wins|draws|avg goals|both scored") {
            $data.H2H += $lines[$i].Trim()
        }
    }
    
    return $data
}

# Función para extraer Team Trends
function Get-TeamTrends {
    param([string]$Url)
    
    Write-Host "=== EXTRAYENDO TEAM TRENDS ===" -ForegroundColor Cyan
    
    $teamTrendsUrl = "$Url`?tab=team-trends"
    $result = Invoke-RestMethod -Uri "$ApiBase/page-content?url=$teamTrendsUrl" -TimeoutSec 60
    
    $lines = $result.contentPreview -split "`n"
    $trends = @()
    
    $inTrend = $false
    $currentTrend = @{}
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match "Goals|Corners|Cards|BTTS|Clean sheet|Scoring|Shots") {
            if ($currentTrend.Count -gt 0) {
                $trends += $currentTrend
            }
            $currentTrend = @{
                Tipo = $lines[$i].Trim()
                Datos = @()
            }
            $inTrend = $true
        }
        
        if ($inTrend) {
            if ($lines[$i] -match "Opp\. hits|H2H|Hit|Rate|Avg|League avg" -or 
                $lines[$i] -match "^\d+/\d+$" -or 
                $lines[$i] -match "^\d+\.\d+$" -or 
                $lines[$i] -match "^\d+%$") {
                $currentTrend.Datos += $lines[$i].Trim()
            }
        }
    }
    
    if ($currentTrend.Count -gt 0) {
        $trends += $currentTrend
    }
    
    return $trends
}

# Función para extraer Player Trends
function Get-PlayerTrends {
    param([string]$Url)
    
    Write-Host "=== EXTRAYENDO PLAYER TRENDS ===" -ForegroundColor Cyan
    
    $playerTrendsUrl = "$Url`?tab=player-trends"
    $result = Invoke-RestMethod -Uri "$ApiBase/page-content?url=$playerTrendsUrl" -TimeoutSec 60
    
    $lines = $result.contentPreview -split "`n"
    $players = @()
    
    $currentPlayer = ""
    $currentTrends = @()
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i].Trim()
        
        # Detectar nombre de jugador
        if ($line -match "^\d+\w+$" -and $line -match "^\d" -and $line.Length -gt 2) {
            if ($currentPlayer -ne "" -and $currentTrends.Count -gt 0) {
                $players += [PSCustomObject]@{
                    Nombre = $currentPlayer
                    Trends = $currentTrends
                }
            }
            $currentPlayer = $line
            $currentTrends = @()
        }
        
        # Detectar trends
        if ($line -match "Goal|Assist|Shot|Card|Foul|Pass|Tackle|Header|Involvement") {
            $currentTrends += $line
        }
        
        # Detectar porcentajes y ratios
        if ($line -match "^\d+/\d+$" -or $line -match "^\d+%$") {
            $currentTrends += $line
        }
    }
    
    if ($currentPlayer -ne "" -and $currentTrends.Count -gt 0) {
        $players += [PSCustomObject]@{
            Nombre = $currentPlayer
            Trends = $currentTrends
        }
    }
    
    return $players
}

# Función para presentar resultados
function Show-Analysis {
    param(
        [string]$MatchName,
        [hashtable]$Overview,
        [array]$TeamTrends,
        [array]$PlayerTrends
    )
    
    Write-Host "`n========================================" -ForegroundColor Yellow
    Write-Host "  ANÁLISIS: $MatchName" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    
    # Overview
    Write-Host "`n### OVERVIEW ###" -ForegroundColor Green
    Write-Host "Alineaciones:" -ForegroundColor White
    $Overview.Alineaciones | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    
    Write-Host "`nRecent Form:" -ForegroundColor White
    $Overview.RecentForm | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    
    Write-Host "`nH2H:" -ForegroundColor White
    $Overview.H2H | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    
    # Team Trends
    Write-Host "`n### TEAM TRENDS ###" -ForegroundColor Green
    foreach ($trend in $TeamTrends) {
        Write-Host "Trend: $($trend.Tipo)" -ForegroundColor Yellow
        $trend.Datos | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    }
    
    # Player Trends
    Write-Host "`n### PLAYER TRENDS ###" -ForegroundColor Green
    foreach ($player in $PlayerTrends) {
        Write-Host "Jugador: $($player.Nombre)" -ForegroundColor Yellow
        Write-Host "  Trends: $($player.Trends -join ', ')" -ForegroundColor Gray
    }
}

# FLUJO PRINCIPAL
Write-Host "=== INICIANDO ANÁLISIS ===" -ForegroundColor Cyan
Write-Host "Fixture URL: $FixtureUrl" -ForegroundColor Yellow

# Extraer datos
$overviewData = Get-OverviewData -Url $FixtureUrl
$teamTrendsData = Get-TeamTrends -Url $FixtureUrl
$playerTrendsData = Get-PlayerTrends -Url $FixtureUrl

# Presentar resultados
$matchName = ($FixtureUrl -split "/")[-1] -replace "-", " "
Show-Analysis -MatchName $matchName -Overview $overviewData -TeamTrends $teamTrendsData -PlayerTrends $playerTrendsData

Write-Host "`n=== ANÁLISIS COMPLETADO ===" -ForegroundColor Green
