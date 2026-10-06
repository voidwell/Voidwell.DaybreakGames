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
| OAuth2 / OpenID Connect authority (the Voidwell auth server) | n/a | Validates incoming JWT and reference tokens; configured through the `Auth` section |
| Docker | optional | Container build and deployment |

NuGet versions are managed centrally in [Directory.Packages.props](Directory.Packages.props).

## Configuration

Settings are read from `appsettings.json`, then `appsettings.{Environment}.json` (for example `appsettings.Development.json`, optional), then environment variables. Environment variables override files; use `__` for nested keys, for example `Auth__ClientSecret` for `Auth:ClientSecret`.

| Key | Required | Description |
|---|---|---|
| `DBConnectionString` | Yes | Npgsql connection string, e.g. `Server=localhost;Database=voidwell.daybreakgames;Username=...;Password=...` |
| `PoolSize` | No | DbContext pool size (default `100`) |
| `CensusServiceKey` | Yes | Census service ID (without the `s:` prefix) |
| `CensusServiceNamespace` | Yes | Census namespace, normally `ps2` |
| `Auth:Authority` | Yes | Base URL of the token authority used to validate JWTs and introspect reference tokens. The app fails at startup if the `Auth` section is missing |
| `Auth:ClientId` | Yes | Client (API resource) id used for token introspection |
| `Auth:ClientSecret` | Yes | Secret for that client |
| `Auth:RoleClaimType` | No | Claim type that carries user roles |
| `ApplicationName` | No | Name used in logs and Swagger (`Voidwell.DaybreakGames` in `appsettings.json`) |
| `RedisConfiguration` | No | StackExchange.Redis connection string. Empty keeps the cache in memory only |
| `OriginAddress` | No | Extra allowed CORS origin (`http://localhost:4200` is always allowed) |
| `CensusWebsocketServices` | No | Comma-separated event names to subscribe to (e.g. `Death, FacilityControl, PlayerLogin`). Defaults are in `appsettings.json`. Empty disables event processing |
| `CensusWebsocketExperienceIds` | No | Experience IDs to subscribe to |
| `CensusWebsocketWorlds` / `CensusWebsocketCharacters` | No | Worlds and characters to stream; `all` by default |
| `DisableUpdater` | No | `true` disables the scheduled Census store updater |
| `DisableCharacterUpdater` | No | `true` disables the background character updater |
| `DisableCensusMonitor` | No | `true` disables the Census websocket monitor |
| `LogCensusErrors` | No | `true` logs Census client errors (default `false`) |
| `Serilog` | No | Standard Serilog configuration section (minimum level and per-namespace overrides); defaults are in `appsettings.json` |

Example `appsettings.Development.json` (placed in `src/Voidwell.DaybreakGames.Api/`; it is gitignored, so keep real secrets there):

```json
{
  "DBConnectionString": "Server=localhost;Database=voidwell.daybreakgames;Username=postgres;Password=postgres",
  "CensusServiceKey": "your-service-id",
  "CensusServiceNamespace": "ps2",
  "Auth": {
    "Authority": "https://auth.example.com",
    "ClientId": "voidwell-daybreakgames",
    "ClientSecret": "dev-secret"
  },
  "RedisConfiguration": "localhost:6379",
  "CensusWebsocketServices": "",
  "DisableUpdater": true
}
```

The EF design-time factory reads `DBConnectionString` from `appsettings.json` and `appsettings.{Environment}.json` (default `Development`) in the working directory of the Data project, then environment variables, so migration commands need an `appsettings.Development.json` in `src/Voidwell.DaybreakGames.Data/` too.

### Logging

Logging, caching, authentication and Swagger come from the shared `Voidwell.Common.*` packages (Logging, Cache, Authentication, Swagger). Logging uses Serilog configured from the `Serilog` section. In Development it writes readable text to the console; otherwise it writes compact JSON (each event carries an `Application` property from `ApplicationName`).

## Running

```bash
dotnet run --project src/Voidwell.DaybreakGames.Api
```

The API listens on `http://0.0.0.0:5000`. Pending EF migrations are applied on startup.

## Database migrations

Migrations are applied automatically when the API starts. To add one, use the helper script (it wraps `dotnet ef` and uses the `Development` environment):

- `scripts/init-migrate.sh [name]`: add a new migration (timestamped unless a name is given)

## Tests

Tests use xunit v3 on Microsoft Testing Platform (enabled through `global.json`):

```bash
dotnet test --solution Voidwell.DaybreakGames.slnx
```

Each application project has a matching test project named `<Project>.Test` under `test/` (Census, CensusStore, Domain, Live, Services, Utils). Shared test helpers (mock extensions, an in-memory `ICache`, map test data) live in `test/Shared`, and common packages are set once in `test/Directory.Build.props`. Run a single project with `dotnet test --project test/Voidwell.DaybreakGames.Live.Test`.

## Docker

```bash
docker build -t voidwell-daybreakgames .
docker run -p 5000:5000 -e DBConnectionString=... -e CensusServiceKey=... voidwell-daybreakgames
```

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
| `Voidwell.DaybreakGames.Utils` | Shared helpers and hosted-service management |

## License

[MIT](LICENSE)