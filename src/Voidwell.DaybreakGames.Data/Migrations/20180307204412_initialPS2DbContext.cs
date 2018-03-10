using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class initialPS2DbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert",
                columns: table => new
                {
                    world_id = table.Column<int>(nullable: false),
                    metagame_instance_id = table.Column<int>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: true),
                    last_faction_nc = table.Column<float>(nullable: true),
                    last_faction_tr = table.Column<float>(nullable: true),
                    last_faction_vs = table.Column<float>(nullable: true),
                    metagame_event_id = table.Column<int>(nullable: true),
                    start_date = table.Column<DateTime>(nullable: true),
                    start_faction_nc = table.Column<float>(nullable: true),
                    start_faction_tr = table.Column<float>(nullable: true),
                    start_faction_vs = table.Column<float>(nullable: true),
                    zone_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alert", x => new { x.world_id, x.metagame_instance_id });
                });

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    battle_rank = table.Column<int>(nullable: false),
                    battle_rank_percent_to_next = table.Column<int>(nullable: false),
                    certs_earned = table.Column<int>(nullable: false),
                    faction_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    title_id = table.Column<int>(nullable: false),
                    world_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_update_queue",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_update_queue", x => x.character_id);
                });

            migrationBuilder.CreateTable(
                name: "event_achievement_earned",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    achievement_id = table.Column<int>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_achievement_earned", x => new { x.character_id, x.timestamp });
                });

            migrationBuilder.CreateTable(
                name: "event_battlerank_up",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    battle_rank = table.Column<int>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_battlerank_up", x => new { x.character_id, x.timestamp });
                });

            migrationBuilder.CreateTable(
                name: "event_continent_lock",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false),
                    metagame_event_id = table.Column<int>(nullable: true),
                    population_nc = table.Column<float>(nullable: true),
                    population_tr = table.Column<float>(nullable: true),
                    population_vs = table.Column<float>(nullable: true),
                    triggering_faction = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_continent_lock", x => new { x.timestamp, x.world_id, x.zone_id });
                });

            migrationBuilder.CreateTable(
                name: "event_continent_unlock",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false),
                    metagame_event_id = table.Column<int>(nullable: true),
                    triggering_faction = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_continent_unlock", x => new { x.timestamp, x.world_id, x.zone_id });
                });

            migrationBuilder.CreateTable(
                name: "event_death",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    attacker_character_id = table.Column<string>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    attacker_fire_mode_id = table.Column<int>(nullable: true),
                    attacker_loadout_id = table.Column<int>(nullable: true),
                    attacker_outfit_id = table.Column<string>(nullable: true),
                    attacker_vehicle_id = table.Column<int>(nullable: true),
                    attacker_weapon_id = table.Column<int>(nullable: true),
                    character_loadout_id = table.Column<int>(nullable: true),
                    character_outfit_id = table.Column<string>(nullable: true),
                    is_headshot = table.Column<bool>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_death", x => new { x.timestamp, x.attacker_character_id, x.character_id });
                });

            migrationBuilder.CreateTable(
                name: "event_facility_control",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    facility_id = table.Column<int>(nullable: false),
                    duration_held = table.Column<int>(nullable: false),
                    new_faction_id = table.Column<int>(nullable: true),
                    old_faction_id = table.Column<int>(nullable: true),
                    outfit_id = table.Column<string>(nullable: true),
                    zone_control_nc = table.Column<float>(nullable: true),
                    zone_control_tr = table.Column<float>(nullable: true),
                    zone_control_vs = table.Column<float>(nullable: true),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_facility_control", x => new { x.timestamp, x.world_id, x.facility_id });
                });

            migrationBuilder.CreateTable(
                name: "event_gain_experience",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    experience_id = table.Column<int>(nullable: false),
                    amount = table.Column<int>(nullable: false),
                    loadout_id = table.Column<int>(nullable: true),
                    other_id = table.Column<string>(nullable: true),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_gain_experience", x => new { x.timestamp, x.character_id, x.experience_id });
                });

            migrationBuilder.CreateTable(
                name: "event_metagame_event",
                columns: table => new
                {
                    metagame_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    experience_bonus = table.Column<int>(nullable: true),
                    instance_id = table.Column<int>(nullable: true),
                    metagame_event_id = table.Column<int>(nullable: true),
                    metagame_event_state = table.Column<string>(nullable: true),
                    timestamp = table.Column<DateTime>(nullable: false),
                    world_id = table.Column<int>(nullable: false),
                    zone_control_nc = table.Column<float>(nullable: true),
                    zone_control_tr = table.Column<float>(nullable: true),
                    zone_control_vs = table.Column<float>(nullable: true),
                    zone_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_metagame_event", x => x.metagame_id);
                });

            migrationBuilder.CreateTable(
                name: "event_player_facility_capture",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    facility_id = table.Column<int>(nullable: false),
                    outfit_id = table.Column<string>(nullable: true),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_player_facility_capture", x => new { x.timestamp, x.character_id, x.facility_id });
                });

            migrationBuilder.CreateTable(
                name: "event_player_facility_defend",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    facility_id = table.Column<int>(nullable: false),
                    outfit_id = table.Column<string>(nullable: true),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_player_facility_defend", x => new { x.timestamp, x.character_id, x.facility_id });
                });

            migrationBuilder.CreateTable(
                name: "event_player_login",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    world_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_player_login", x => new { x.timestamp, x.character_id });
                });

            migrationBuilder.CreateTable(
                name: "event_player_logout",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    world_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_player_logout", x => new { x.timestamp, x.character_id });
                });

            migrationBuilder.CreateTable(
                name: "event_vehicle_destroy",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(nullable: false),
                    attacker_character_id = table.Column<string>(nullable: false),
                    character_id = table.Column<string>(nullable: false),
                    attacker_vehicle_id = table.Column<int>(nullable: false),
                    vehicle_id = table.Column<int>(nullable: false),
                    attacker_loadout_id = table.Column<int>(nullable: true),
                    attacker_weapon_id = table.Column<int>(nullable: true),
                    facility_id = table.Column<int>(nullable: true),
                    faction_id = table.Column<int>(nullable: true),
                    world_id = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_event_vehicle_destroy", x => new { x.timestamp, x.attacker_character_id, x.character_id, x.attacker_vehicle_id, x.vehicle_id });
                });

            migrationBuilder.CreateTable(
                name: "facility_link",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    facility_id_a = table.Column<int>(nullable: false),
                    facility_id_b = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_facility_link", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "faction",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    code_tag = table.Column<string>(nullable: true),
                    image_id = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    user_selectable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_faction", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    faction_id = table.Column<int>(nullable: true),
                    image_id = table.Column<int>(nullable: true),
                    is_vehicle_weapon = table.Column<bool>(nullable: false),
                    item_category_id = table.Column<int>(nullable: true),
                    item_type_id = table.Column<int>(nullable: true),
                    max_stack_size = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_category",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_item_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "map_hex",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    hex_type = table.Column<int>(nullable: false),
                    map_region_id = table.Column<int>(nullable: false),
                    type_name = table.Column<string>(nullable: true),
                    x_pos = table.Column<int>(nullable: false),
                    y_pos = table.Column<int>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_map_hex", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "map_region",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    facility_id = table.Column<int>(nullable: false),
                    facility_name = table.Column<string>(nullable: true),
                    facility_type = table.Column<string>(nullable: true),
                    facility_type_id = table.Column<int>(nullable: false),
                    x_pos = table.Column<float>(nullable: false),
                    y_pos = table.Column<float>(nullable: false),
                    z_pos = table.Column<float>(nullable: false),
                    zone_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_map_region", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "metagame_event_category",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    experience_bonus = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_metagame_event_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "metagame_event_state",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_metagame_event_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outfit",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    alias = table.Column<string>(nullable: true),
                    created_date = table.Column<DateTime>(nullable: false),
                    faction_id = table.Column<int>(nullable: true),
                    leader_character_id = table.Column<string>(nullable: true),
                    member_count = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    world_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outfit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_session",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    character_id = table.Column<string>(nullable: false),
                    duration = table.Column<int>(nullable: false),
                    login_date = table.Column<DateTime>(nullable: false),
                    logout_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_player_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    faction_id = table.Column<int>(nullable: false),
                    image_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    profile_type_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_profile", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sanctioned_weapon",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sanctioned_weapon", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "title",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_title", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "updater_scheduler",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    last_update_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_updater_scheduler", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    cost = table.Column<int>(nullable: false),
                    cost_resource_id = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    image_id = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicle", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "world",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_world", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "zone",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    code = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    hex_size = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_zone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_lifetime_stat",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    achievement_count = table.Column<int>(nullable: true, defaultValue: 0),
                    assist_count = table.Column<int>(nullable: true, defaultValue: 0),
                    domination_count = table.Column<int>(nullable: true, defaultValue: 0),
                    facility_capture_count = table.Column<int>(nullable: true, defaultValue: 0),
                    facility_defended_count = table.Column<int>(nullable: true, defaultValue: 0),
                    medal_count = table.Column<int>(nullable: true, defaultValue: 0),
                    revenge_count = table.Column<int>(nullable: true, defaultValue: 0),
                    skill_points = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_given = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_taken_by = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_deaths = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_fire_count = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_headshots = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_hit_count = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_kills = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_play_time = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_score = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_vehicle_kills = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_lifetime_stat", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_character_lifetime_stat_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_lifetime_stat_by_faction",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    domination_count_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    domination_count_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    domination_count_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    facility_capture_count_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    facility_capture_count_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    facility_capture_count_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    revenge_count_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    revenge_count_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    revenge_count_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_given_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_given_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_given_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_taken_by_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_taken_by_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_damage_taken_by_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_headshots_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_headshots_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_headshots_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_killed_by_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_killed_by_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_killed_by_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_kills_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_kills_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_kills_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_vehicle_kills_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_vehicle_kills_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    weapon_vehicle_kills_vs = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_lifetime_stat_by_faction", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_character_lifetime_stat_by_faction_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_stat",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    profile_id = table.Column<int>(nullable: false),
                    deaths = table.Column<int>(nullable: true, defaultValue: 0),
                    fire_count = table.Column<int>(nullable: true, defaultValue: 0),
                    hit_count = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by = table.Column<int>(nullable: true, defaultValue: 0),
                    kills = table.Column<int>(nullable: true, defaultValue: 0),
                    play_time = table.Column<int>(nullable: true, defaultValue: 0),
                    score = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_stat", x => new { x.character_id, x.profile_id });
                    table.ForeignKey(
                        name: "f_k_character_stat_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_stat_by_faction",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    profile_id = table.Column<int>(nullable: false),
                    killed_by_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_vs = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_stat_by_faction", x => new { x.character_id, x.profile_id });
                    table.ForeignKey(
                        name: "f_k_character_stat_by_faction_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_time",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    created_date = table.Column<DateTime>(nullable: false),
                    last_login_date = table.Column<DateTime>(nullable: false),
                    last_save_date = table.Column<DateTime>(nullable: false),
                    minutes_played = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_time", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_character_time_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_weapon_stat",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    item_id = table.Column<int>(nullable: false),
                    vehicle_id = table.Column<int>(nullable: false),
                    damage_given = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_taken_by = table.Column<int>(nullable: true, defaultValue: 0),
                    deaths = table.Column<int>(nullable: true, defaultValue: 0),
                    fire_count = table.Column<int>(nullable: true, defaultValue: 0),
                    headshots = table.Column<int>(nullable: true, defaultValue: 0),
                    hit_count = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by = table.Column<int>(nullable: true, defaultValue: 0),
                    kills = table.Column<int>(nullable: true, defaultValue: 0),
                    play_time = table.Column<int>(nullable: true, defaultValue: 0),
                    score = table.Column<int>(nullable: true, defaultValue: 0),
                    vehicle_kills = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_weapon_stat", x => new { x.character_id, x.item_id, x.vehicle_id });
                    table.ForeignKey(
                        name: "f_k_character_weapon_stat_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_weapon_stat_by_faction",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    item_id = table.Column<int>(nullable: false),
                    vehicle_id = table.Column<int>(nullable: false),
                    damage_given_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_given_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_given_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_taken_by_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_taken_by_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    damage_taken_by_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    headshots_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    headshots_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    headshots_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    killed_by_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    kills_vs = table.Column<int>(nullable: true, defaultValue: 0),
                    vehicle_kills_nc = table.Column<int>(nullable: true, defaultValue: 0),
                    vehicle_kills_tr = table.Column<int>(nullable: true, defaultValue: 0),
                    vehicle_kills_vs = table.Column<int>(nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_character_weapon_stat_by_faction", x => new { x.character_id, x.item_id, x.vehicle_id });
                    table.ForeignKey(
                        name: "f_k_character_weapon_stat_by_faction_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outfit_member",
                columns: table => new
                {
                    character_id = table.Column<string>(nullable: false),
                    member_since_date = table.Column<DateTime>(nullable: true),
                    outfit_id = table.Column<string>(nullable: false),
                    rank = table.Column<string>(nullable: true),
                    rank_ordinal = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_outfit_member", x => x.character_id);
                    table.ForeignKey(
                        name: "f_k_outfit_member_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_faction",
                columns: table => new
                {
                    vehicle_id = table.Column<int>(nullable: false),
                    faction_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicle_faction", x => new { x.vehicle_id, x.faction_id });
                    table.ForeignKey(
                        name: "f_k_vehicle_faction_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_character_weapon_stat_kills",
                table: "character_weapon_stat",
                column: "kills");

            migrationBuilder.CreateIndex(
                name: "i_x_event_death_attacker_character_id_character_id",
                table: "event_death",
                columns: new[] { "attacker_character_id", "character_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_player_session_character_id_login_date_logout_date",
                table: "player_session",
                columns: new[] { "character_id", "login_date", "logout_date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert");

            migrationBuilder.DropTable(
                name: "character_lifetime_stat");

            migrationBuilder.DropTable(
                name: "character_lifetime_stat_by_faction");

            migrationBuilder.DropTable(
                name: "character_stat");

            migrationBuilder.DropTable(
                name: "character_stat_by_faction");

            migrationBuilder.DropTable(
                name: "character_time");

            migrationBuilder.DropTable(
                name: "character_update_queue");

            migrationBuilder.DropTable(
                name: "character_weapon_stat");

            migrationBuilder.DropTable(
                name: "character_weapon_stat_by_faction");

            migrationBuilder.DropTable(
                name: "event_achievement_earned");

            migrationBuilder.DropTable(
                name: "event_battlerank_up");

            migrationBuilder.DropTable(
                name: "event_continent_lock");

            migrationBuilder.DropTable(
                name: "event_continent_unlock");

            migrationBuilder.DropTable(
                name: "event_death");

            migrationBuilder.DropTable(
                name: "event_facility_control");

            migrationBuilder.DropTable(
                name: "event_gain_experience");

            migrationBuilder.DropTable(
                name: "event_metagame_event");

            migrationBuilder.DropTable(
                name: "event_player_facility_capture");

            migrationBuilder.DropTable(
                name: "event_player_facility_defend");

            migrationBuilder.DropTable(
                name: "event_player_login");

            migrationBuilder.DropTable(
                name: "event_player_logout");

            migrationBuilder.DropTable(
                name: "event_vehicle_destroy");

            migrationBuilder.DropTable(
                name: "facility_link");

            migrationBuilder.DropTable(
                name: "faction");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "item_category");

            migrationBuilder.DropTable(
                name: "map_hex");

            migrationBuilder.DropTable(
                name: "map_region");

            migrationBuilder.DropTable(
                name: "metagame_event_category");

            migrationBuilder.DropTable(
                name: "metagame_event_state");

            migrationBuilder.DropTable(
                name: "outfit");

            migrationBuilder.DropTable(
                name: "outfit_member");

            migrationBuilder.DropTable(
                name: "player_session");

            migrationBuilder.DropTable(
                name: "profile");

            migrationBuilder.DropTable(
                name: "sanctioned_weapon");

            migrationBuilder.DropTable(
                name: "title");

            migrationBuilder.DropTable(
                name: "updater_scheduler");

            migrationBuilder.DropTable(
                name: "vehicle_faction");

            migrationBuilder.DropTable(
                name: "world");

            migrationBuilder.DropTable(
                name: "zone");

            migrationBuilder.DropTable(
                name: "character");

            migrationBuilder.DropTable(
                name: "vehicle");
        }
    }
}
