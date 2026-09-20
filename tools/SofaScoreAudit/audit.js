const https = require('https');
const fs = require('fs');

const h = { rejectUnauthorized: false };
const baseUrl = 'https://localhost:7000/api/BrowserTest/fetch?apiPath=';

const seasons = {
    48078: 'Liga 1 2023',
    57741: 'Liga 1 2024',
    70962: 'Liga 1 2025',
    88529: 'Liga 1 2026'
};

const allEvents = [];

function fetch(path) {
    return new Promise((resolve, reject) => {
        https.get(baseUrl + encodeURIComponent(path), h, res => {
            let d = '';
            res.on('data', c => d += c);
            res.on('end', () => {
                try { resolve(JSON.parse(d)); }
                catch(e) { reject(new Error(`Parse error`)); }
            });
        }).on('error', reject);
    });
}

function normalize(name) {
    return name.toLowerCase().trim()
        .replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i')
        .replace(/ó/g, 'o').replace(/ú/g, 'u').replace(/ñ/g, 'n')
        .replace(/\s+/g, ' ');
}

function fuzzyMatch(a, b) {
    const na = normalize(a);
    const nb = normalize(b);
    if (na === nb) return true;
    if (na.includes(nb) || nb.includes(na)) return true;
    // Check if first word matches (e.g., "deportivo" in both)
    const wa = na.split(' ');
    const wb = nb.split(' ');
    if (wa[0] === wb[0] && wa.length > 1 && wb.length > 1) return true;
    return false;
}

async function main() {
    for (const [seasonId, seasonName] of Object.entries(seasons)) {
        process.stdout.write(`Fetching ${seasonName}...`);
        const roundsResp = await fetch(`unique-tournament/406/season/${seasonId}/rounds`);
        const rounds = roundsResp.rounds || [];
        let eventCount = 0;
        for (const r of rounds) {
            try {
                const eventsResp = await fetch(`unique-tournament/406/season/${seasonId}/events/round/${r.round}/prefix/${encodeURIComponent(r.prefix)}`);
                const events = eventsResp.events || [];
                for (const evt of events) {
                    const date = new Date(evt.startTimestamp * 1000).toISOString().split('T')[0];
                    allEvents.push({
                        date,
                        home: evt.homeTeam.name,
                        away: evt.awayTeam.name,
                        homeScore: evt.homeScore?.current || 0,
                        awayScore: evt.awayScore?.current || 0
                    });
                    eventCount++;
                }
            } catch {}
        }
        console.log(` ${eventCount} events`);
    }
    console.log(`\nTotal SofaScore: ${allEvents.length}`);

    const sample = [
        ['2023-02-25', 'Deportivo Garcilaso', 'Atlético Grau'],
        ['2023-03-24', 'Universitario', 'Cienciano'],
        ['2023-03-26', 'Deportivo Municipal', 'Unión Comercio'],
        ['2023-05-28', 'Unión Comercio', 'ADT'],
        ['2023-07-03', 'César Vallejo', 'Cienciano'],
        ['2023-08-16', 'Carlos Manucci', 'Real Garcilaso'],
        ['2023-08-16', 'Deportivo Garcilaso', 'Sport Boys'],
        ['2023-08-17', 'Deportivo Binacional', 'César Vallejo'],
        ['2023-08-19', 'Universitario', 'Deportivo Garcilaso'],
        ['2023-08-26', 'Carlos Manucci', 'Cienciano'],
        ['2023-09-17', 'ADT', 'Sport Huancayo'],
        ['2024-02-02', 'Real Garcilaso', 'UTC Cajamarca'],
        ['2024-02-12', 'Unión Comercio', 'Universitario'],
        ['2024-02-26', 'ADT', 'Deportivo Municipal'],
        ['2024-03-26', 'Deportivo Municipal', 'Unión Comercio'],
        ['2024-09-17', 'ADT', 'Sport Huancayo'],
        ['2024-09-24', 'Melgar', 'Unión Comercio'],
        ['2025-02-17', 'Ayacucho', 'Alianza Universidad'],
        ['2025-02-27', 'ADT', 'Sport Boys'],
        ['2025-03-07', 'Alianza Lima', 'Ayacucho'],
        ['2025-05-18', 'Melgar', 'Atlético Grau'],
        ['2025-10-16', 'Alianza Lima', 'Sport Boys'],
        ['2026-02-15', 'Universitario', 'ADT'],
        ['2026-02-23', 'UCV Moquegua', 'Deportivo Garcilaso'],
        ['2026-02-28', 'Sport Boys', 'UCV Moquegua'],
    ];

    // CSV data with scores (from the original import)
    const csvScores = [
        ['2023-02-25', 'Deportivo Garcilaso', 'Atlético Grau', 1, 0],
        ['2023-03-24', 'Universitario', 'Cienciano', 3, 0],
        ['2023-03-26', 'Deportivo Municipal', 'Unión Comercio', 2, 0],
        ['2023-05-28', 'Unión Comercio', 'ADT', 4, 3],
        ['2023-07-03', 'César Vallejo', 'Cienciano', 1, 1],
        ['2023-08-16', 'Carlos Manucci', 'Real Garcilaso', 0, 0],
        ['2023-08-16', 'Deportivo Garcilaso', 'Sport Boys', 3, 0],
        ['2023-08-17', 'Deportivo Binacional', 'César Vallejo', 1, 0],
        ['2023-08-19', 'Universitario', 'Deportivo Garcilaso', 1, 1],
        ['2023-08-26', 'Carlos Manucci', 'Cienciano', 2, 0],
        ['2023-09-17', 'ADT', 'Sport Huancayo', 2, 1],
        ['2024-02-02', 'Real Garcilaso', 'UTC Cajamarca', 1, 1],
        ['2024-02-12', 'Unión Comercio', 'Universitario', 1, 0],
        ['2024-02-26', 'ADT', 'Deportivo Municipal', 3, 0],
        ['2024-03-26', 'Deportivo Municipal', 'Unión Comercio', 2, 0],
        ['2024-09-17', 'ADT', 'Sport Huancayo', 2, 1],
        ['2024-09-24', 'Melgar', 'Unión Comercio', 3, 1],
        ['2025-02-17', 'Ayacucho', 'Alianza Universidad', 3, 2],
        ['2025-02-27', 'ADT', 'Sport Boys', 2, 2],
        ['2025-03-07', 'Alianza Lima', 'Ayacucho', 2, 0],
        ['2025-05-18', 'Melgar', 'Atlético Grau', 1, 1],
        ['2025-10-16', 'Alianza Lima', 'Sport Boys', 3, 1],
        ['2026-02-15', 'Universitario', 'ADT', 2, 0],
        ['2026-02-23', 'UCV Moquegua', 'Deportivo Garcilaso', 1, 0],
        ['2026-02-28', 'Sport Boys', 'UCV Moquegua', 0, 0],
    ];

    console.log('\n=== AUDITORÍA COMPLETA vs SofaScore ===\n');

    let exactMatch = 0, fuzzyOnly = 0, notFound = 0, scoreCorrect = 0, scoreWrong = 0;

    for (let i = 0; i < sample.length; i++) {
        const [date, home, away] = sample[i];
        const [,, , csvHomeScore, csvAwayScore] = csvScores[i];

        // Try exact match first
        let match = allEvents.find(e =>
            e.date === date && normalize(e.home) === normalize(home) && normalize(e.away) === normalize(away)
        );

        let matchType = 'EXACT';
        if (!match) {
            // Try fuzzy match
            match = allEvents.find(e =>
                e.date === date && fuzzyMatch(e.home, home) && fuzzyMatch(e.away, away)
            );
            matchType = 'FUZZY';
        }

        if (!match) {
            notFound++;
            console.log(`  MISS | ${date} | ${home} vs ${away} | CSV: ${csvHomeScore}-${csvAwayScore}`);
            continue;
        }

        if (matchType === 'EXACT') exactMatch++;
        else fuzzyOnly++;

        const scoreOk = match.homeScore === csvHomeScore && match.awayScore === csvAwayScore;
        if (scoreOk) scoreCorrect++;
        else scoreWrong++;

        const tag = matchType === 'EXACT' ? 'OK  ' : 'FUZZ';
        const scoreTag = scoreOk ? 'SCORE_OK' : `SCORE_ERR SofaScore:${match.homeScore}-${match.awayScore} CSV:${csvHomeScore}-${csvAwayScore}`;
        console.log(`  ${tag} | ${date} | ${home} vs ${away} | ${scoreTag}`);
    }

    console.log('\n=== RESUMEN ===');
    console.log(`Muestra: 25`);
    console.log(`Match exacto: ${exactMatch}/25 (${Math.round(exactMatch*100/25)}%)`);
    console.log(`Match fuzzy (nombres diferentes): ${fuzzyOnly}/25 (${Math.round(fuzzyOnly*100/25)}%)`);
    console.log(`Total encontrado: ${exactMatch+fuzzyOnly}/25 (${Math.round((exactMatch+fuzzyOnly)*100/25)}%)`);
    console.log(`No encontrado: ${notFound}/25`);
    console.log(`Score correcto (CSV vs SofaScore): ${scoreCorrect}/${exactMatch+fuzzyOnly}`);
    console.log(`Score incorrecto: ${scoreWrong}/${exactMatch+fuzzyOnly}`);

    // Team name mapping discrepancies
    console.log('\n=== DISCREPANCIAS DE NOMBRE ===');
    const nameMap = {};
    for (const [date, home, away] of sample) {
        const match = allEvents.find(e => e.date === date && fuzzyMatch(e.home, home) && fuzzyMatch(e.away, away));
        if (match) {
            if (normalize(match.home) !== normalize(home)) {
                const key = `${home} -> ${match.home}`;
                nameMap[key] = (nameMap[key] || 0) + 1;
            }
            if (normalize(match.away) !== normalize(away)) {
                const key = `${away} -> ${match.away}`;
                nameMap[key] = (nameMap[key] || 0) + 1;
            }
        }
    }
    for (const [k, v] of Object.entries(nameMap).sort((a,b) => b[1] - a[1])) {
        console.log(`  ${k} (${v}x)`);
    }

    fs.writeFileSync('audit-results.json', JSON.stringify({
        total: 25, exactMatch, fuzzyOnly, notFound, scoreCorrect, scoreWrong,
        nameMap
    }, null, 2));
}

main().catch(console.error);
