$fetchBase = "http://localhost:5272/api/BrowserTest/fetch"
$matchIds = @(16280806, 16280801, 16280815, 16280791, 16280784, 16281128, 16281116, 16153077, 16147473, 16022831, 16022826, 15930114, 15929229, 15881788, 15885659, 15714336, 15655437)

foreach ($id in $matchIds) {
    $statsUri = "$fetchBase`?apiPath=event/$id/statistics"
    $evtUri = "$fetchBase`?apiPath=event/$id"
    $stats = Invoke-RestMethod -Uri $statsUri
    $evt = Invoke-RestMethod -Uri $evtUri
    
    $homeName = $evt.event.homeTeam.name
    $awayName = $evt.event.awayTeam.name
    $date = ([DateTimeOffset]::FromUnixTimeSeconds($evt.event.startTimestamp)).ToString("yyyy-MM-dd")
    $isHome = $homeName -match "Sporting Cristal"
    
    foreach ($period in $stats.statistics) {
        if ($period.period -eq "1ST" -or $period.period -eq "ALL") {
            $allGroups = $period.groups
            $cornerVal = $null
            $shotsVal = $null
            
            foreach ($group in $allGroups) {
                foreach ($item in $group.statisticsItems) {
                    if ($item.key -eq "cornerKicks") {
                        $cornerVal = "$($item.home)-$($item.away)"
                    }
                    if ($item.key -eq "totalShotsOnGoal") {
                        $shotsVal = "$($item.home)-$($item.away)"
                    }
                }
            }
            
            Write-Output "$date|$homeName|$awayName|$isHome|$($period.period)|Corners:$cornerVal|Shots:$shotsVal"
        }
    }
}
