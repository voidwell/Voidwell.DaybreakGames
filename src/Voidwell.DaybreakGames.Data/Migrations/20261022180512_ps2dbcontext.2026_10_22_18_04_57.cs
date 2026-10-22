using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.DaybreakGames.Data.Migrations
{
    /// <inheritdoc />
    public partial class ps2dbcontext2026_10_22_18_04_57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Converting populated tables in place rewrites every row under an exclusive lock and needs
            // twice the disk space. Large databases are converted with the upgrade runbook instead, which
            // applies this migration to empty tables and copies the data in afterwards.
            migrationBuilder.Sql($"""
                DO $$
                BEGIN
                    IF (SELECT sum(pg_total_relation_size(oid)) FROM pg_class WHERE relkind = 'r' AND relnamespace = 'public'::regnamespace) > {_maxInPlaceConversionBytes} THEN
                        RAISE EXCEPTION 'Database is too large to convert character ids to bigint in place. Follow the PostgreSQL upgrade runbook.';
                    END IF;
                END $$;
                """);

            migrationBuilder.DropTable(
                name: "event_gain_experience");

            migrationBuilder.DropTable(
                name: "experience");

            migrationBuilder.Sql("DELETE FROM updater_scheduler WHERE id = 'ExperienceStore';");

            migrationBuilder.DropIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy");

            migrationBuilder.DropIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death");

            // Npgsql cannot express text -> bigint (it needs USING), and the foreign keys to
            // character(id) have to come off while both sides change type.
            DropCharacterForeignKeys(migrationBuilder);

            foreach (var (table, column, nullable) in _idColumns)
            {
                var value = nullable ? $"NULLIF({column}, '')" : column;
                migrationBuilder.Sql($"ALTER TABLE \"{table}\" ALTER COLUMN {column} TYPE bigint USING {value}::bigint;");
            }

            AddCharacterForeignKeys(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropCharacterForeignKeys(migrationBuilder);

            foreach (var (table, column, _) in _idColumns)
            {
                migrationBuilder.Sql($"ALTER TABLE \"{table}\" ALTER COLUMN {column} TYPE text USING {column}::text;");
            }

            AddCharacterForeignKeys(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "event_gain_experience",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    character_id = table.Column<string>(type: "text", nullable: false),
                    experience_id = table.Column<int>(type: "integer", nullable: false),
                    loadout_id = table.Column<int>(type: "integer", nullable: true),
                    other_id = table.Column<string>(type: "text", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    world_id = table.Column<int>(type: "integer", nullable: false),
                    zone_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_gain_experience", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "experience",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    xp = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_experience", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_event_vehicle_destroy_attacker_weapon_id_timestamp",
                table: "event_vehicle_destroy",
                columns: new[] { "attacker_weapon_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_death_attacker_weapon_id_timestamp",
                table: "event_death",
                columns: new[] { "attacker_weapon_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_character_id_experience_id",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "character_id", "experience_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_event_gain_experience_timestamp_world_id_experience_id_zone",
                table: "event_gain_experience",
                columns: new[] { "timestamp", "world_id", "experience_id", "zone_id" });
        }

        private const long _maxInPlaceConversionBytes = 1L * 1024 * 1024 * 1024;

        private static readonly (string Table, string Column, bool Nullable)[] _idColumns =
        [
            ("player_session", "character_id", false),
            ("outfit_member", "outfit_id", false),
            ("outfit_member", "character_id", false),
            ("outfit", "leader_character_id", true),
            ("outfit", "id", false),
            ("event_vehicle_destroy", "character_id", false),
            ("event_vehicle_destroy", "attacker_character_id", false),
            ("event_player_logout", "character_id", false),
            ("event_player_login", "character_id", false),
            ("event_player_facility_defend", "outfit_id", true),
            ("event_player_facility_defend", "character_id", false),
            ("event_player_facility_capture", "outfit_id", true),
            ("event_player_facility_capture", "character_id", false),
            ("event_facility_control", "outfit_id", true),
            ("event_death", "character_outfit_id", true),
            ("event_death", "attacker_outfit_id", true),
            ("event_death", "character_id", false),
            ("event_death", "attacker_character_id", false),
            ("event_battlerank_up", "character_id", false),
            ("event_achievement_earned", "character_id", false),
            ("character_weapon_stat_by_faction", "character_id", false),
            ("character_weapon_stat", "character_id", false),
            ("character_update_queue", "character_id", false),
            ("character_time", "character_id", false),
            ("character_stat_history", "character_id", false),
            ("character_stat_by_faction", "character_id", false),
            ("character_stat", "character_id", false),
            ("character_rating", "character_id", false),
            ("character_lifetime_stat_by_faction", "character_id", false),
            ("character_lifetime_stat", "character_id", false),
            ("character_directive_tree", "character_id", false),
            ("character_directive_tier", "character_id", false),
            ("character_directive_objective", "character_id", false),
            ("character_directive", "character_id", false),
            ("character_achievement", "character_id", false),
            ("character", "id", false),
        ];

        // Tables whose character_id references character(id)
        private static readonly string[] _characterForeignKeyTables =
        [
            "character_lifetime_stat",
            "character_lifetime_stat_by_faction",
            "character_stat",
            "character_stat_by_faction",
            "character_time",
            "character_weapon_stat",
            "character_weapon_stat_by_faction",
            "outfit_member",
            "character_stat_history",
            "character_rating",
            "character_directive_tree",
            "character_directive",
            "character_directive_tier",
            "character_directive_objective",
        ];

        private static void DropCharacterForeignKeys(MigrationBuilder migrationBuilder)
        {
            foreach (var table in _characterForeignKeyTables)
            {
                migrationBuilder.DropForeignKey(
                    name: $"f_k_{table}_character_character_id",
                    table: table);
            }
        }

        private static void AddCharacterForeignKeys(MigrationBuilder migrationBuilder)
        {
            foreach (var table in _characterForeignKeyTables)
            {
                migrationBuilder.AddForeignKey(
                    name: $"f_k_{table}_character_character_id",
                    table: table,
                    column: "character_id",
                    principalTable: "character",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            }
        }
    }
}
