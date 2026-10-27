-- Merges the database of a former single-platform instance (PS4 US or PS4 EU) into the merged database.
-- Run through scripts/merge-platform-database.sh, which sets the psql variables used here.
--
-- Copied from the source: worlds (tagged with the source's platform), characters and everything stored
-- per character, outfits, sessions, alerts, map snapshots, daily populations and stream events.
-- Kept from the target: static Census data (identical on every platform), weapon aggregates and daily weapon
-- stats, sanctioned weapons, updater history and migration history.
--
-- Ids never collide across platforms, so existing rows win on conflict. Rows with generated ids (player sessions,
-- metagame events) get new ids, so a platform can only be merged once; platform_merge_log enforces that.

\set ON_ERROR_STOP on

BEGIN;

CREATE TABLE IF NOT EXISTS platform_merge_log (
    platform varchar(16) PRIMARY KEY,
    source_database text NOT NULL,
    merged_at timestamptz NOT NULL DEFAULT now()
);

SELECT EXISTS (SELECT 1 FROM platform_merge_log WHERE platform = :'platform') AS already_merged \gset
\if :already_merged
    \echo 'Platform' :platform 'was already merged; see platform_merge_log.'
    DO $$ BEGIN RAISE EXCEPTION 'Platform already merged'; END $$;
\endif

CREATE EXTENSION IF NOT EXISTS postgres_fdw;

-- Dropping the imported tables lists every one of them otherwise
SET client_min_messages = warning;

DROP SERVER IF EXISTS platform_merge_source CASCADE;
CREATE SERVER platform_merge_source FOREIGN DATA WRAPPER postgres_fdw
    OPTIONS (host :'source_host', port :'source_port', dbname :'source_db');
CREATE USER MAPPING FOR CURRENT_USER SERVER platform_merge_source
    OPTIONS (user :'source_user', password :'source_password');

DROP SCHEMA IF EXISTS platform_merge_source CASCADE;
CREATE SCHEMA platform_merge_source;
IMPORT FOREIGN SCHEMA public FROM SERVER platform_merge_source INTO platform_merge_source;

SET client_min_messages = notice;

-- The source predates the platform column, so its worlds take the platform being merged
INSERT INTO public.world (id, name, is_active, platform)
SELECT id, name, is_active, :'platform'
FROM platform_merge_source.world
ON CONFLICT (id) DO UPDATE SET platform = EXCLUDED.platform;

DO $merge$
DECLARE
    -- Parents before children: the character tables reference character
    merge_tables text[] := ARRAY[
        'character',
        'character_time',
        'character_lifetime_stat',
        'character_lifetime_stat_by_faction',
        'character_stat',
        'character_stat_by_faction',
        'character_stat_history',
        'character_weapon_stat',
        'character_weapon_stat_by_faction',
        'character_achievement',
        'character_directive_tree',
        'character_directive_tier',
        'character_directive',
        'character_directive_objective',
        'character_rating',
        'character_update_queue',
        'outfit',
        'outfit_member',
        'player_session',
        'alert',
        'zone_ownership_snapshot',
        'daily_population',
        'event_achievement_earned',
        'event_battlerank_up',
        'event_continent_lock',
        'event_continent_unlock',
        'event_death',
        'event_facility_control',
        'event_metagame_event',
        'event_player_facility_capture',
        'event_player_facility_defend',
        'event_player_login',
        'event_player_logout',
        'event_vehicle_destroy'
    ];
    merge_table text;
    columns text;
    missing_columns text;
    row_count bigint;
BEGIN
    FOREACH merge_table IN ARRAY merge_tables LOOP
        -- Generated ids are left to the target's identity sequence
        SELECT string_agg(quote_ident(target.column_name), ', ' ORDER BY target.ordinal_position),
               string_agg(target.column_name, ', ') FILTER (WHERE source.column_name IS NULL)
        INTO columns, missing_columns
        FROM information_schema.columns target
        LEFT JOIN information_schema.columns source
            ON source.table_schema = 'platform_merge_source'
            AND source.table_name = target.table_name
            AND source.column_name = target.column_name
        WHERE target.table_schema = 'public'
            AND target.table_name = merge_table
            AND target.is_identity = 'NO';

        IF columns IS NULL THEN
            RAISE EXCEPTION 'Table % does not exist in the target database', merge_table;
        END IF;

        IF missing_columns IS NOT NULL THEN
            RAISE EXCEPTION 'Source table % is missing columns: %. Migrate the source database to the same release first.', merge_table, missing_columns;
        END IF;

        EXECUTE format('INSERT INTO public.%I (%s) SELECT %s FROM platform_merge_source.%I ON CONFLICT DO NOTHING',
            merge_table, columns, columns, merge_table);

        GET DIAGNOSTICS row_count = ROW_COUNT;
        RAISE NOTICE '%: % rows merged', merge_table, row_count;
    END LOOP;
END
$merge$;

INSERT INTO platform_merge_log (platform, source_database) VALUES (:'platform', :'source_db');

SET client_min_messages = warning;

DROP SCHEMA platform_merge_source CASCADE;
DROP SERVER platform_merge_source CASCADE;

COMMIT;
