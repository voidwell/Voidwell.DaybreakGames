CREATE OR REPLACE FUNCTION public.update_daily_weapon_stats(calcdate date DEFAULT NULL::date)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
        weaponCategoryIds integer[] := array[2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 17, 18, 19, 20, 21, 22, 23, 24, 102, 104, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 138, 144];
        aircraftVehicleIds integer[] := array[7, 8, 9, 10, 11, 14, 2019];
        lowerBoundDate date;
        upperBoundDate date;
BEGIN
        lowerBoundDate := COALESCE(calcDate, (timezone('utc', now()) - interval '1d')::date);
        upperBoundDate := (lowerBoundDate + interval '1d')::date;

        INSERT INTO daily_weapon_stats ("date", "weapon_id", "uniques", "avg_br", "kills", "headshots", "kpu", "q1_kpu", "q2_kpu", "q3_kpu", "q4_kpu", "q4_headshots", "q4_kills", "q4_uniques")
                SELECT
                        date_trunc('day', d."timestamp")::timestamp without time zone AS "date",
                        d."attacker_weapon_id",
                        COALESCE(count(distinct(d."attacker_character_id")), 0) AS "uniques",
                        COALESCE(AVG(c."battle_rank"), 0) AS "avg_br",
                        count(*) AS "kills",
                        COALESCE(SUM(CASE WHEN d."is_headshot" IS TRUE THEN 1 ELSE 0 END), 0) AS "headshots",
                        COALESCE((CAST(count(*) AS float) / NULLIF(CAST(count(distinct(d."attacker_character_id")) AS float), 0)), 0) AS "kpu",
                        COALESCE((CAST(SUM(CASE WHEN c."battle_rank" BETWEEN 1 AND 30 THEN 1 ELSE 0 END) AS float) / NULLIF(CAST(count(distinct(CASE WHEN c."battle_rank" BETWEEN 1 AND 30 THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "q1_kpu",
                        COALESCE((CAST(SUM(CASE WHEN c."battle_rank" BETWEEN 31 AND 60 THEN 1 ELSE 0 END) AS float) / NULLIF(CAST(count(distinct(CASE WHEN c."battle_rank" BETWEEN 31 AND 60 THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "q2_kpu",
                        COALESCE((CAST(SUM(CASE WHEN c."battle_rank" BETWEEN 61 AND 90 THEN 1 ELSE 0 END) AS float) / NULLIF(CAST(count(distinct(CASE WHEN c."battle_rank" BETWEEN 61 AND 90 THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "q3_kpu",
                        COALESCE((CAST(SUM(CASE WHEN c."battle_rank" BETWEEN 91 AND 120 THEN 1 ELSE 0 END) AS float) / NULLIF(CAST(count(distinct(CASE WHEN c."battle_rank" BETWEEN 91 AND 120 THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "q4_kpu",
                        COALESCE(SUM(CASE WHEN d."is_headshot" IS TRUE AND c."battle_rank" BETWEEN 91 AND 120 THEN 1 ELSE 0 END), 0) AS "q4_headshots",
                        COALESCE(SUM(CASE WHEN c."battle_rank" BETWEEN 91 AND 120 THEN 1 ELSE 0 END), 0) AS "q4_kills",
                        count(distinct(CASE WHEN c."battle_rank" BETWEEN 91 AND 120 THEN d."attacker_character_id" END)) AS "q4_uniques"
                FROM event_death AS d
                LEFT OUTER JOIN character AS c ON d."attacker_character_id" = c."id"
                LEFT OUTER JOIN item AS i ON d."attacker_weapon_id" = i."id"
                WHERE d."timestamp" >= lowerBoundDate AND d."timestamp" < upperBoundDate  AND d."attacker_weapon_id" IS NOT NULL AND i."item_category_id" = ANY(weaponCategoryIds)
                GROUP BY 2, 1
                ORDER BY 2, 1 DESC
        ON CONFLICT ("date", "weapon_id") DO UPDATE
                SET ("uniques", "avg_br", "kills", "headshots", "kpu", "q1_kpu", "q2_kpu", "q3_kpu", "q4_kpu", "q4_headshots", "q4_kills", "q4_uniques") =
                (EXCLUDED."uniques", EXCLUDED."avg_br", EXCLUDED."kills", EXCLUDED."headshots", EXCLUDED."kpu", EXCLUDED."q1_kpu", EXCLUDED."q2_kpu", EXCLUDED."q3_kpu", EXCLUDED."q4_kpu", EXCLUDED."q4_headshots", EXCLUDED."q4_kills", EXCLUDED."q4_uniques");

        INSERT INTO daily_weapon_stats ("date", "weapon_id", "vehicle_kills", "aircraft_kills", "vehicle_kpu", "aircraft_kpu")
                SELECT
                        date_trunc('day', d."timestamp")::timestamp without time zone AS "date",
                        d."attacker_weapon_id",
                        COALESCE(SUM(CASE WHEN d."vehicle_id" IS NOT NULL AND d."vehicle_id" != 0 AND d."vehicle_id" != ANY(aircraftVehicleIds) THEN 1 ELSE 0 END), 0) AS "vehicle_kills",
                        COALESCE(SUM(CASE WHEN d."vehicle_id" = ANY(aircraftVehicleIds) THEN 1 ELSE 0 END), 0) AS "aircraft_kills",
                        COALESCE((CAST(COALESCE(SUM(CASE WHEN d."vehicle_id" IS NOT NULL  AND d."vehicle_id" != 0 AND d."vehicle_id" != ANY(aircraftVehicleIds) THEN 1 ELSE 0 END), 0) AS float) / NULLIF(CAST(count(distinct(CASE WHEN d."vehicle_id" IS NOT NULL AND d."vehicle_id" != ANY(aircraftVehicleIds) THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "vehicle_kpu",
                        COALESCE((CAST(COALESCE(SUM(CASE WHEN d."vehicle_id" = ANY(aircraftVehicleIds) THEN 1 ELSE 0 END), 0) AS float) / NULLIF(CAST(count(distinct(CASE WHEN d."vehicle_id" = ANY(aircraftVehicleIds) THEN d."attacker_character_id" END)) AS float), 0)), 0) AS "aircraft_kpu"
                FROM event_vehicle_destroy AS d
                LEFT OUTER JOIN item AS i ON d."attacker_weapon_id" = i."id"
                WHERE d."timestamp" >= lowerBoundDate AND d."timestamp" < upperBoundDate AND d."attacker_weapon_id" IS NOT NULL AND i."item_category_id"  = ANY(weaponCategoryIds)
                GROUP BY 2, 1
                ORDER BY 2, 1 DESC
:
