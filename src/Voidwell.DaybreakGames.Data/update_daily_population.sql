CREATE OR REPLACE FUNCTION public.update_daily_population(calcdate date DEFAULT NULL::date)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
        lowerBoundDate date;
        upperBoundDate date;
BEGIN
        lowerBoundDate := COALESCE(calcDate, (timezone('utc', now()) - interval '1d')::date);
        upperBoundDate := (lowerBoundDate + interval '1d')::date;

        INSERT INTO daily_population ("date", "world_id", "vs_count", "nc_count", "tr_count", "vs_avg_play_time", "nc_avg_play_time", "tr_avg_play_time", "avg_play_time")
                SELECT
                        date_trunc('day', d."login_date")::timestamp without time zone AS "date",
                        c."world_id",
                        COUNT(DISTINCT(CASE WHEN c."faction_id" = 1 THEN d."character_id" END)),
                        COUNT(DISTINCT(CASE WHEN c."faction_id" = 2 THEN d."character_id" END)),
                        COUNT(DISTINCT(CASE WHEN c."faction_id" = 3 THEN d."character_id" END)),
                        COALESCE(AVG(CASE WHEN c."faction_id" = 1 THEN d."duration" END), 0),
                        COALESCE(AVG(CASE WHEN c."faction_id" = 2 THEN d."duration" END), 0),
                        COALESCE(AVG(CASE WHEN c."faction_id" = 3 THEN d."duration" END), 0),
                        COALESCE(AVG(d."duration"), 0)
                FROM player_session AS d
                LEFT OUTER JOIN character AS c ON d."character_id" = c."id"
                WHERE d."login_date" >= lowerBoundDate AND d."login_date" < upperBoundDate AND c."id" IS NOT NULL
                GROUP BY 2, 1
                ORDER BY 2, 1 DESC
        ON CONFLICT ("date", "world_id") DO UPDATE
                SET ("vs_count", "nc_count", "tr_count", "vs_avg_play_time", "nc_avg_play_time", "tr_avg_play_time", "avg_play_time") =
                (EXCLUDED."vs_count", EXCLUDED."nc_count", EXCLUDED."tr_count", EXCLUDED."vs_avg_play_time", EXCLUDED."nc_avg_play_time", EXCLUDED."tr_avg_play_time", EXCLUDED."avg_play_time");
END; $function$
