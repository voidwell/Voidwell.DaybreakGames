# Voidwell.DaybreakGames

[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/voidwell/voidwell.daybreakgames/build-test.yml?branch=main&style=for-the-badge)](https://github.com/voidwell/voidwell.daybreakgames/actions/workflows/build-test.yml)
[![Latest Release](https://img.shields.io/github/v/release/voidwell/voidwell.daybreakgames?style=for-the-badge)](https://github.com/voidwell/voidwell.daybreakgames/releases/latest)
[![MIT License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

Backend API for Voidwell's PlanetSide 2 data. It ingests Daybreak Games' Census REST and event-stream APIs, stores the results in PostgreSQL, caches in Redis, and serves them over an ASP.NET Core HTTP API.

## Requirements

| Dependency | Version | Purpose |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 (`net10.0`) | Build and run |
| PostgreSQL | any supported release | Primary data store (EF Core + Npgsql); migrations run automatically on startup |
| Redis | optional | Shared cache (FusionCache L2 + backplane) and list storage. If `RedisConfiguration` is empty, caching is in-memory per instance |
| Daybreak Games Census service ID | n/a | Access to the Census API and event stream |
| Voidwell auth server (`https://auth.voidwell.com`) | n/a | Validates incoming JWT / reference tokens and issues client-credentials tokens used to look up user roles |
| Docker | optional | Container build and deployment |

NuGet versions are managed centrally in [Directory.Packages.props](Directory.Packages.props).

## Configuration

Settings are read from `appsettings.json`, then `devsettings.json` (Development environment only, optional), then environment variables. Environment variables override files; use `__` for nesting if needed.

| Key | Required | Description |
|---|---|---|
| `DBConnectionString` | Yes | Npgsql connection string, e.g. `Server=localhost;Database=voidwell.daybreakgames;Username=...;Password=...` |
| `CensusServiceKey` | Yes | Census service ID (without the `s:` prefix) |
| `CensusServiceNamespace` | Yes | Census namespace, normally `ps2` |
| `ApiResourceSecret` | Yes | Secret for this API's resource (`voidwell-daybreakgames`), used for token introspection |
| `ClientSecret` | Yes | Client secret used to request tokens for the `voidwell-usermanagement` scope |
| `RedisConfiguration` | No | StackExchange.Redis connection string. Empty keeps the cache in memory only. List keys are prefixed `Voidwell.DaybreakGames_` |
| `OriginAddress` | No | Extra allowed CORS origin (`http://localhost:4200` is always allowed) |
| `CensusWebsocketServices` | No | Comma-separated event names to subscribe to (e.g. `Death, FacilityControl, PlayerLogin`). Defaults are in `appsettings.json`. Empty disables event processing |
| `CensusWebsocketExperienceIds` | No | Experience IDs to subscribe to |
| `CensusWebsocketWorlds` / `CensusWebsocketCharacters` | No | Worlds and characters to stream; `all` by default |
| `DisableUpdater` | No | `true` disables the scheduled Census store updater |
| `LogCensusErrors` | No | `true` logs Census client errors (default `false`) |
| `Serilog` | No | Standard Serilog configuration section (levels and overrides) in `appsettings.json` |

Example `devsettings.json` (placed in `src/Voidwell.DaybreakGames.Api/`; it is not committed with real secrets):

```json
{
  "DBConnectionString": "Server=localhost;Database=voidwell.daybreakgames;Username=postgres;Password=postgres",
  "CensusServiceKey": "your-service-id",
  "CensusServiceNamespace": "ps2",
  "ApiResourceSecret": "dev-secret",
  "ClientSecret": "dev-secret",
  "RedisConfiguration": "localhost:6379",
  "CensusWebsocketServices": "",
  "DisableUpdater": true
}
```

The EF design-time factory reads `DBConnectionString` from a `devsettings.json` in the working directory of the Data project, so migration commands need that file in `src/Voidwell.DaybreakGames.Data/` too.

### Logging

Logging uses Serilog configured from the `Serilog` section. In Development it writes readable text to the console; otherwise it writes compact JSON (`Application` is set to `Voidwell.DaybreakGames`).

## Running

```bash
dotnet run --project src/Voidwell.DaybreakGames.Api
```

The API listens on `http://0.0.0.0:5000`. Pending EF migrations are applied on startup.

## Database migrations

Windows helper scripts in the repo root wrap `dotnet ef`:

- `init-migrate.bat`: add a new timestamped migration
- `init-update.bat`: apply migrations to the configured database

## Tests

Tests use xunit v3 on Microsoft Testing Platform (enabled through `global.json`):

```bash
dotnet test --solution Voidwell.DaybreakGames.slnx
```

Each application project has a matching test project named `<Project>.Test` under `test/` (Cache, Census, CensusStore, Domain, Live, Services, Utils). Shared test helpers (mock extensions, an in-memory `ICache`, map test data) live in `test/Shared`, and common packages are set once in `test/Directory.Build.props`. Run a single project with `dotnet test --project test/Voidwell.DaybreakGames.Live.Test`.

## Docker

```bash
docker build -t voidwell-daybreakgames .
docker run -p 5000:5000 -e DBConnectionString=... -e CensusServiceKey=... voidwell-daybreakgames
```

`Dockerfile.debug` builds a development image with `vsdbg` and SSH for remote debugging.

## Project layout

| Project | Role |
|---|---|
| `Voidwell.DaybreakGames.Api` | ASP.NET Core host, controllers, authentication |
| `Voidwell.DaybreakGames.Services` | Application services and mapping |
| `Voidwell.DaybreakGames.Live` | Census event-stream processing and live game state |
| `Voidwell.DaybreakGames.CensusStore` | Census-to-database stores and scheduled updater |
| `Voidwell.DaybreakGames.Census` | Census API collections and models |
| `Voidwell.DaybreakGames.Data` | EF Core context, repositories, migrations |
| `Voidwell.DaybreakGames.Domain` | Domain models |
| `Voidwell.DaybreakGames.Cache` | `ICache` over FusionCache (memory + optional Redis) |
| `Voidwell.DaybreakGames.Utils` | Shared helpers and hosted-service management |

## License

[MIT](LICENSE)