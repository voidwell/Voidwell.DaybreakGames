#!/usr/bin/env bash
# Merges the database of a former PS4 US or PS4 EU instance into the merged database.
# Stop the source instance first, and back up the target: the merge runs in one transaction but can't be undone after it commits.
#
# Usage: scripts/merge-platform-database.sh <PS4US|PS4EU>
#
# Environment:
#   TARGET_DATABASE_URL   psql connection string for the merged database, e.g. postgresql://user:pass@host:5432/voidwell.daybreakgames
#                         The user needs to be able to create the postgres_fdw extension and a foreign server.
#   SOURCE_HOST           Host of the source database, as reached from the target database server
#   SOURCE_PORT           Port of the source database (default 5432)
#   SOURCE_DB             Name of the source database
#   SOURCE_USER           User for the source database
#   SOURCE_PASSWORD       Password for that user
set -euo pipefail

platform="${1:-}"
case "$platform" in
    PS4US|PS4EU) ;;
    *)
        echo "Usage: $0 <PS4US|PS4EU>" >&2
        exit 1
        ;;
esac

: "${TARGET_DATABASE_URL:?TARGET_DATABASE_URL is required}"
: "${SOURCE_HOST:?SOURCE_HOST is required}"
: "${SOURCE_DB:?SOURCE_DB is required}"
: "${SOURCE_USER:?SOURCE_USER is required}"
: "${SOURCE_PASSWORD:?SOURCE_PASSWORD is required}"

psql "$TARGET_DATABASE_URL" \
    -v platform="$platform" \
    -v source_host="$SOURCE_HOST" \
    -v source_port="${SOURCE_PORT:-5432}" \
    -v source_db="$SOURCE_DB" \
    -v source_user="$SOURCE_USER" \
    -v source_password="$SOURCE_PASSWORD" \
    -f "$(dirname "${BASH_SOURCE[0]}")/merge-platform-database.sql"

echo "Merged $platform. Restart the API and clear its Redis keys so cached worlds and lookups are rebuilt."
