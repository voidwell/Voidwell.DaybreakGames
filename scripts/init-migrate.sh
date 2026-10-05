#!/usr/bin/env bash
# Adds a new EF Core migration for PS2DbContext.
# Usage: scripts/init-migrate.sh [migration-name]
# Without a name, the migration is named ps2dbcontext.<timestamp>.
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/../src/Voidwell.DaybreakGames.Data"

export ASPNETCORE_ENVIRONMENT=Development

migration="${1:-ps2dbcontext.$(date +%Y_%m_%d_%H_%M_%S)}"

dotnet ef migrations add "$migration" -v \
    -c Voidwell.DaybreakGames.Data.PS2DbContext \
    -o ./Migrations
