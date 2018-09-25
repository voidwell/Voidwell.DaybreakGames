CREATE OR REPLACE FUNCTION public.update_weapon_aggregate()
 RETURNS void
 LANGUAGE plpgsql
AS $BODY$
BEGIN
        INSERT INTO weapon_aggregate ("item_id", "vehicle_id", "avg_kills", "std_kills", "sum_kills", "avg_deaths", "std_deaths", "sum_deaths", "avg_fire_count", "std_fire_count", "sum_fire_count", "avg_hit_count", "std_hit_count", "sum_hit_count", "avg_headshots", "std_headshots", "sum_headshots", "avg_play_time", "std_play_time", "sum_play_time", "avg_score", "std_score", "sum_score", "avg_vehicle_kills", "std_vehicle_kills", "sum_vehicle_kills", "avg_kdr", "std_kdr", "avg_accuracy", "std_accuracy", "avg_hsr", "std_hsr", "avg_kph", "std_kph", "avg_vkph", "std_vkph")
                SELECT
                         t.item_id,
                         t.vehicle_id,
                         CAST(AVG(t.kills) AS float) AS avg_kills,
                         CAST(STDDEV(t.kills) AS float) AS std_kills,
                         CAST(SUM(t.kills) AS bigint) AS sum_kills,
                         CAST(AVG(t.deaths) AS float) AS avg_deaths,
                         CAST(STDDEV(t.deaths) AS float) AS std_deaths,
                         CAST(SUM(t.deaths) AS bigint) AS sum_deaths,
                         CAST(AVG(t.fire_count) AS float) AS avg_fire_count,
                         CAST(STDDEV(t.fire_count) AS float) AS std_fire_count,
                         CAST(SUM(t.fire_count) AS bigint) AS sum_fire_count,
                         CAST(AVG(t.hit_count) AS float) AS avg_hit_count,
                         CAST(STDDEV(t.hit_count) AS float) AS std_hit_count,
                         CAST(SUM(t.hit_count) AS bigint) AS sum_hit_count,
                         CAST(AVG(t.headshots) AS float) AS avg_headshots,
                         CAST(STDDEV(t.headshots) AS float) AS std_headshots,
                         CAST(SUM(t.headshots) AS bigint) AS sum_headshots,
                         CAST(AVG(t.play_time) AS float) AS avg_play_time,
                         CAST(STDDEV(t.play_time) AS float) AS std_play_time,
                         CAST(SUM(t.play_time) AS bigint) AS sum_play_time,
                         CAST(AVG(t.score) AS float) AS avg_score,
                         CAST(STDDEV(t.score) AS float) AS std_score,
                         CAST(SUM(t.score) AS bigint) AS sum_score,
                         CAST(AVG(t.vehicle_kills) AS float) AS avg_vehicle_kills,
                         CAST(STDDEV(t.vehicle_kills) AS float) AS std_vehicle_kills,
                         CAST(SUM(t.vehicle_kills) AS bigint) AS sum_vehicle_kills,
                         CAST(AVG(t.kills/ CAST(t.deaths AS float)) AS float) AS avg_kdr,
                         CAST(STDDEV(t.kills/ CAST(t.deaths AS float)) AS float) AS std_kdr,
                         CAST(AVG(t.hit_count/ CAST(t.fire_count AS float)) AS float) AS avg_accu,
                         CAST(STDDEV(t.hit_count/ CAST(t.fire_count as float)) AS float) AS std_accu,
                         CAST(AVG(t.headshots/ CAST(t.kills AS float)) AS float) AS avg_hsr,
                         CAST(STDDEV(t.headshots/ CAST(t.kills AS float)) AS float) AS std_hsr,
                         CAST(AVG(t.kills/( CAST(t.play_time AS float) / 3600)) AS float) AS avg_kph,
                         CAST(STDDEV(t.kills/( CAST(t.play_time AS float) / 3600)) AS float) AS std_kph,
                         CAST(AVG(t.vehicle_kills/( CAST(t.play_time AS float) / 3600)) AS float) AS avg_vkph,
                         CAST(STDDEV(t.vehicle_kills/( CAST(t.play_time AS float) / 3600)) AS float) AS std_vkph
                FROM character_weapon_stat AS t
                WHERE t."kills" > 250 AND t."deaths" > 0 AND t."fire_count" > 0 AND t."play_time" > 0 AND (t."kills" / CAST(t."deaths" AS float)) <= 100
                GROUP BY t."item_id", t."vehicle_id"
        ON CONFLICT ("item_id", "vehicle_id") DO UPDATE
                SET ("avg_kills", "std_kills", "sum_kills", "avg_deaths", "std_deaths", "sum_deaths", "avg_fire_count", "std_fire_count", "sum_fire_count", "avg_hit_count", "std_hit_count", "sum_hit_count", "avg_headshots", "std_headshots", "sum_headshots", "avg_play_time", "std_play_time", "sum_play_time", "avg_score", "std_score", "sum_score", "avg_vehicle_kills", "std_vehicle_kills", "sum_vehicle_kills", "avg_kdr", "std_kdr", "avg_accuracy", "std_accuracy", "avg_hsr", "std_hsr", "avg_kph", "std_kph", "avg_vkph", "std_vkph") =
                (EXCLUDED."avg_kills", EXCLUDED."std_kills", EXCLUDED."sum_kills", EXCLUDED."avg_deaths", EXCLUDED."std_deaths", EXCLUDED."sum_deaths", EXCLUDED."avg_fire_count", EXCLUDED."std_fire_count", EXCLUDED."sum_fire_count", EXCLUDED."avg_hit_count", EXCLUDED."std_hit_count", EXCLUDED."sum_hit_count", EXCLUDED."avg_headshots", EXCLUDED."std_headshots", EXCLUDED."sum_headshots", EXCLUDED."avg_play_time", EXCLUDED."std_play_time", EXCLUDED."sum_play_time", EXCLUDED."avg_score", EXCLUDED."std_score", EXCLUDED."sum_score", EXCLUDED."avg_vehicle_kills", EXCLUDED."std_vehicle_kills", EXCLUDED."sum_vehicle_kills", EXCLUDED."avg_kdr", EXCLUDED."std_kdr", EXCLUDED."avg_accuracy", EXCLUDED."std_accuracy", EXCLUDED."avg_hsr", EXCLUDED."std_hsr", EXCLUDED."avg_kph", EXCLUDED."std_kph", EXCLUDED."avg_vkph", EXCLUDED."std_vkph");
END;
$BODY$;
