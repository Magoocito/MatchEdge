# MatchEdge

REST API for retrieving and exposing football team statistics, built with ASP.NET Core and Clean Architecture.

## Overview

MatchEdge aggregates team performance data from external sports data providers and exposes it through a versioned HTTP API suitable for downstream analytics, betting models, and operational tooling.

## Architecture

The solution follows a layered Clean Architecture layout:

| Layer | Project | Responsibility |
| --- | --- | --- |
| Presentation | `MatchEdge.Api` | ASP.NET Core Web API, controllers, Swagger |
| Application | `MatchEdge.Application` | Use cases and service contracts |
| Domain | `MatchEdge.Domain` | Core models and business entities |
| Infrastructure | `MatchEdge.Infrastructure` | External integrations and technical adapters |
| Tests | `MatchEdge.UnitTests` | Unit test project (xUnit) |

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Swagger / OpenAPI (Swashbuckle)
- xUnit (test scaffold)
- Visual Studio 2022 / VS Code

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- `curl.exe` available on the system `PATH` (used by the SofaScore HTTP adapter on Windows)

## Getting Started

```bash
# Restore dependencies and build
dotnet build MatchEdge.sln

# Run the API
dotnet run --project src/MatchEdge.Api/MatchEdge.Api.csproj
```

In Development, Swagger UI is available at `/swagger`.

## Configuration

Application settings live under `src/MatchEdge.Api/appsettings.json`:

```json
{
  "SofaScore": {
    "BaseUrl": "https://www.sofascore.com/api/v1/"
  }
}
```

Override values per environment using `appsettings.Development.json` or user secrets for local development.

## API

### Get team statistics

```http
GET /api/TeamStatistics?teamId={teamId}&tournamentId={tournamentId}&seasonId={seasonId}
```

Returns aggregated team statistics for the requested team, tournament, and season. Responds with `404 Not Found` when no data is available.

## Solution Structure

```text
MatchEdge/
├── src/
│   ├── MatchEdge.Api/
│   ├── MatchEdge.Application/
│   ├── MatchEdge.Domain/
│   └── MatchEdge.Infrastructure/
├── tests/
│   └── MatchEdge.UnitTests/
├── MatchEdge.sln
└── README.md
```

## Development Notes

- Keep domain logic free of infrastructure concerns.
- Register new services in `Program.cs` using explicit dependency injection.
- Do not commit build artifacts (`bin/`, `obj/`), IDE state (`.vs/`), or user-specific project files.
