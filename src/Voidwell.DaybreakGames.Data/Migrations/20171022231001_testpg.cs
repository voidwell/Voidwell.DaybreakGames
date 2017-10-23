using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Data.Migrations
{
    public partial class testpg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "CharacterUpdateQueue",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterUpdateQueue", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "EventAchievementEarned",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    AchievementId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAchievementEarned", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "EventBattlerankUp",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    BattleRank = table.Column<int>(type: "int4", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBattlerankUp", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "EventContinentLock",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: false),
                    MetagameEventId = table.Column<string>(type: "text", nullable: true),
                    PopulationNc = table.Column<float>(type: "float4", nullable: false),
                    PopulationTr = table.Column<float>(type: "float4", nullable: false),
                    PopulationVs = table.Column<float>(type: "float4", nullable: false),
                    TriggeringFaction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventContinentLock", x => new { x.Timestamp, x.WorldId, x.ZoneId });
                });

            migrationBuilder.CreateTable(
                name: "EventContinentUnkock",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: false),
                    MetagameEventId = table.Column<string>(type: "text", nullable: true),
                    TriggeringFaction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventContinentUnkock", x => new { x.Timestamp, x.WorldId, x.ZoneId });
                });

            migrationBuilder.CreateTable(
                name: "EventFacilityControl",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<string>(type: "text", nullable: false),
                    DurationHeld = table.Column<int>(type: "int4", nullable: false),
                    NewFactionId = table.Column<string>(type: "text", nullable: true),
                    OldFactionId = table.Column<string>(type: "text", nullable: true),
                    OutfitId = table.Column<string>(type: "text", nullable: true),
                    ZoneControlNc = table.Column<float>(type: "float4", nullable: false),
                    ZoneControlTr = table.Column<float>(type: "float4", nullable: false),
                    ZoneControlVs = table.Column<float>(type: "float4", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFacilityControl", x => new { x.Timestamp, x.WorldId, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "EventGainExperience",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ExperienceId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "int4", nullable: false),
                    LoadoutId = table.Column<string>(type: "text", nullable: true),
                    OtherId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventGainExperience", x => new { x.Timestamp, x.CharacterId, x.ExperienceId });
                });

            migrationBuilder.CreateTable(
                name: "EventMetagameEvent",
                schema: "public",
                columns: table => new
                {
                    MetagameId = table.Column<string>(type: "text", nullable: false),
                    ExperienceBonus = table.Column<int>(type: "int4", nullable: false),
                    InstanceId = table.Column<string>(type: "text", nullable: true),
                    MetagameEventId = table.Column<string>(type: "text", nullable: true),
                    MetagameEventState = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneControlNc = table.Column<float>(type: "float4", nullable: false),
                    ZoneControlTr = table.Column<float>(type: "float4", nullable: false),
                    ZoneControlVs = table.Column<float>(type: "float4", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMetagameEvent", x => x.MetagameId);
                });

            migrationBuilder.CreateTable(
                name: "EventPlayerFacilityCapture",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<string>(type: "text", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPlayerFacilityCapture", x => new { x.Timestamp, x.CharacterId, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "EventPlayerFacilityDefend",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<string>(type: "text", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPlayerFacilityDefend", x => new { x.Timestamp, x.CharacterId, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "EventPlayerLogin",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPlayerLogin", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "EventPlayerLogout",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventPlayerLogout", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "EventVehicleDestroy",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    AttackerCharacterId = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    AttackerLoadoutId = table.Column<string>(type: "text", nullable: true),
                    AttackerVehicleId = table.Column<string>(type: "text", nullable: false),
                    AttackerWeaponId = table.Column<string>(type: "text", nullable: true),
                    FacilityId = table.Column<string>(type: "text", nullable: true),
                    FactionId = table.Column<string>(type: "text", nullable: true),
                    VehicleId = table.Column<string>(type: "text", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVehicleDestroy", x => new { x.Timestamp, x.AttackerCharacterId, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "FacilityLink",
                schema: "public",
                columns: table => new
                {
                    ZoneId = table.Column<string>(type: "text", nullable: false),
                    FacilityIdA = table.Column<string>(type: "text", nullable: false),
                    FacilityIdB = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityLink", x => new { x.ZoneId, x.FacilityIdA, x.FacilityIdB });
                });

            migrationBuilder.CreateTable(
                name: "Faction",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CodeTag = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserSelectable = table.Column<bool>(type: "bool", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemCategory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapHex",
                schema: "public",
                columns: table => new
                {
                    ZoneId = table.Column<string>(type: "text", nullable: false),
                    MapRegionId = table.Column<string>(type: "text", nullable: false),
                    HexType = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<string>(type: "text", nullable: false),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    XPos = table.Column<int>(type: "int4", nullable: false),
                    YPos = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapHex", x => new { x.ZoneId, x.MapRegionId });
                });

            migrationBuilder.CreateTable(
                name: "MapRegion",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<string>(type: "text", nullable: true),
                    FacilityName = table.Column<string>(type: "text", nullable: true),
                    FacilityType = table.Column<string>(type: "text", nullable: true),
                    FacilityTypeId = table.Column<string>(type: "text", nullable: true),
                    XPos = table.Column<float>(type: "float4", nullable: false),
                    YPos = table.Column<float>(type: "float4", nullable: false),
                    ZPos = table.Column<float>(type: "float4", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapRegion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetagameEventCategory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExperienceBonus = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetagameEventCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetagameEventState",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetagameEventState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSession",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "int4", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LogoutDate = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FactionId = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ProfileTypeId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Title",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Title", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<int>(type: "int4", nullable: false),
                    CostResourceId = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "World",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_World", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HexSize = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FactionId = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    IsVehicleWeapon = table.Column<bool>(type: "bool", nullable: false),
                    ItemCategoryId = table.Column<string>(type: "text", nullable: true),
                    ItemTypeId = table.Column<string>(type: "text", nullable: true),
                    MaxStackSize = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_ItemCategory_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalSchema: "public",
                        principalTable: "ItemCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alert",
                schema: "public",
                columns: table => new
                {
                    WorldId = table.Column<string>(type: "text", nullable: false),
                    MetagameInstanceId = table.Column<string>(type: "text", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LastFactionNc = table.Column<float>(type: "float4", nullable: false),
                    LastFactionTr = table.Column<float>(type: "float4", nullable: false),
                    LastFactionVs = table.Column<float>(type: "float4", nullable: false),
                    MetagameEventId = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    StartFactionNc = table.Column<float>(type: "float4", nullable: false),
                    StartFactionTr = table.Column<float>(type: "float4", nullable: false),
                    StartFactionVs = table.Column<float>(type: "float4", nullable: false),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => new { x.WorldId, x.MetagameInstanceId });
                    table.ForeignKey(
                        name: "FK_Alert_MetagameEventCategory_MetagameEventId",
                        column: x => x.MetagameEventId,
                        principalSchema: "public",
                        principalTable: "MetagameEventCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleFaction",
                schema: "public",
                columns: table => new
                {
                    VehicleId = table.Column<string>(type: "text", nullable: false),
                    FactionId = table.Column<string>(type: "text", nullable: false),
                    DbVehicleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFaction", x => new { x.VehicleId, x.FactionId });
                    table.ForeignKey(
                        name: "FK_VehicleFaction_Vehicle_DbVehicleId",
                        column: x => x.DbVehicleId,
                        principalSchema: "public",
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStat",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ProfileId = table.Column<string>(type: "text", nullable: false),
                    AchievementCount = table.Column<int>(type: "int4", nullable: false),
                    AssistCount = table.Column<int>(type: "int4", nullable: false),
                    Deaths = table.Column<int>(type: "int4", nullable: false),
                    DominationCount = table.Column<int>(type: "int4", nullable: false),
                    FacilityCaptureCount = table.Column<int>(type: "int4", nullable: false),
                    FacilityDefendedCount = table.Column<int>(type: "int4", nullable: false),
                    FireCount = table.Column<int>(type: "int4", nullable: false),
                    HitCount = table.Column<int>(type: "int4", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    KilledBy = table.Column<int>(type: "int4", nullable: false),
                    Kills = table.Column<int>(type: "int4", nullable: false),
                    MedalCount = table.Column<int>(type: "int4", nullable: false),
                    PlayTime = table.Column<int>(type: "int4", nullable: false),
                    RevengeCount = table.Column<int>(type: "int4", nullable: false),
                    Score = table.Column<int>(type: "int4", nullable: false),
                    SkillPoints = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageGiven = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageTakenBy = table.Column<int>(type: "int4", nullable: false),
                    WeaponDeaths = table.Column<int>(type: "int4", nullable: false),
                    WeaponFireCount = table.Column<int>(type: "int4", nullable: false),
                    WeaponHeadshots = table.Column<int>(type: "int4", nullable: false),
                    WeaponHitCount = table.Column<int>(type: "int4", nullable: false),
                    WeaponKilledBy = table.Column<int>(type: "int4", nullable: false),
                    WeaponKills = table.Column<int>(type: "int4", nullable: false),
                    WeaponPlayTime = table.Column<int>(type: "int4", nullable: false),
                    WeaponScore = table.Column<int>(type: "int4", nullable: false),
                    WeaponVehicleKills = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStat", x => new { x.CharacterId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_CharacterStat_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStatByFaction",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ProfileId = table.Column<string>(type: "text", nullable: false),
                    DominationCountNC = table.Column<int>(type: "int4", nullable: false),
                    DominationCountTR = table.Column<int>(type: "int4", nullable: false),
                    DominationCountVS = table.Column<int>(type: "int4", nullable: false),
                    FacilityCaptureCountNC = table.Column<int>(type: "int4", nullable: false),
                    FacilityCaptureCountTR = table.Column<int>(type: "int4", nullable: false),
                    FacilityCaptureCountVS = table.Column<int>(type: "int4", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    KilledByNC = table.Column<int>(type: "int4", nullable: false),
                    KilledByTR = table.Column<int>(type: "int4", nullable: false),
                    KilledByVS = table.Column<int>(type: "int4", nullable: false),
                    KillsNC = table.Column<int>(type: "int4", nullable: false),
                    KillsTR = table.Column<int>(type: "int4", nullable: false),
                    KillsVS = table.Column<int>(type: "int4", nullable: false),
                    RevengeCountNC = table.Column<int>(type: "int4", nullable: false),
                    RevengeCountTR = table.Column<int>(type: "int4", nullable: false),
                    RevengeCountVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageGivenNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageGivenTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageGivenVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageTakenByNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageTakenByTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponDamageTakenByVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponHeadshotsNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponHeadshotsTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponHeadshotsVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponKilledByNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponKilledByTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponKilledByVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponKillsNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponKillsTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponKillsVS = table.Column<int>(type: "int4", nullable: false),
                    WeaponVehicleKillsNC = table.Column<int>(type: "int4", nullable: false),
                    WeaponVehicleKillsTR = table.Column<int>(type: "int4", nullable: false),
                    WeaponVehicleKillsVS = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStatByFaction", x => new { x.CharacterId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_CharacterStatByFaction_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTime",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LastSaveDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    MinutesPlayed = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTime", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "CharacterWeaponStat",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    VehicleId = table.Column<string>(type: "text", nullable: false),
                    DamageGiven = table.Column<int>(type: "int4", nullable: false),
                    DamageTakenBy = table.Column<int>(type: "int4", nullable: false),
                    Deaths = table.Column<int>(type: "int4", nullable: false),
                    FireCount = table.Column<int>(type: "int4", nullable: false),
                    Headshots = table.Column<int>(type: "int4", nullable: false),
                    HitCount = table.Column<int>(type: "int4", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    KilledBy = table.Column<int>(type: "int4", nullable: false),
                    Kills = table.Column<int>(type: "int4", nullable: false),
                    PlayTime = table.Column<int>(type: "int4", nullable: false),
                    Score = table.Column<int>(type: "int4", nullable: false),
                    VehicleKills = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterWeaponStat", x => new { x.CharacterId, x.ItemId, x.VehicleId });
                    table.ForeignKey(
                        name: "FK_CharacterWeaponStat_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "public",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterWeaponStat_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "public",
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterWeaponStatByFaction",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ItemId = table.Column<string>(type: "text", nullable: false),
                    VehicleId = table.Column<string>(type: "text", nullable: false),
                    DamageGivenNC = table.Column<int>(type: "int4", nullable: false),
                    DamageGivenTR = table.Column<int>(type: "int4", nullable: false),
                    DamageGivenVS = table.Column<int>(type: "int4", nullable: false),
                    DamageTakenByNC = table.Column<int>(type: "int4", nullable: false),
                    DamageTakenByTR = table.Column<int>(type: "int4", nullable: false),
                    DamageTakenByVS = table.Column<int>(type: "int4", nullable: false),
                    HeadshotsNC = table.Column<int>(type: "int4", nullable: false),
                    HeadshotsTR = table.Column<int>(type: "int4", nullable: false),
                    HeadshotsVS = table.Column<int>(type: "int4", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    KilledByNC = table.Column<int>(type: "int4", nullable: false),
                    KilledByTR = table.Column<int>(type: "int4", nullable: false),
                    KilledByVS = table.Column<int>(type: "int4", nullable: false),
                    KillsNC = table.Column<int>(type: "int4", nullable: false),
                    KillsTR = table.Column<int>(type: "int4", nullable: false),
                    KillsVS = table.Column<int>(type: "int4", nullable: false),
                    VehicleKillsNC = table.Column<int>(type: "int4", nullable: false),
                    VehicleKillsTR = table.Column<int>(type: "int4", nullable: false),
                    VehicleKillsVS = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterWeaponStatByFaction", x => new { x.CharacterId, x.ItemId, x.VehicleId });
                    table.ForeignKey(
                        name: "FK_CharacterWeaponStatByFaction_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "public",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterWeaponStatByFaction_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "public",
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventDeath",
                schema: "public",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    AttackerCharacterId = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    AttackerFireModeId = table.Column<string>(type: "text", nullable: true),
                    AttackerLoadoutId = table.Column<string>(type: "text", nullable: true),
                    AttackerOutfitId = table.Column<string>(type: "text", nullable: true),
                    AttackerVehicleId = table.Column<string>(type: "text", nullable: true),
                    AttackerWeaponId = table.Column<string>(type: "text", nullable: true),
                    CharacterLoadoutId = table.Column<string>(type: "text", nullable: true),
                    CharacterOutfitId = table.Column<string>(type: "text", nullable: true),
                    IsHeadshot = table.Column<bool>(type: "bool", nullable: false),
                    WorldId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDeath", x => new { x.Timestamp, x.AttackerCharacterId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_EventDeath_Vehicle_AttackerVehicleId",
                        column: x => x.AttackerVehicleId,
                        principalSchema: "public",
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventDeath_Item_AttackerWeaponId",
                        column: x => x.AttackerWeaponId,
                        principalSchema: "public",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Outfit",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    FactionId = table.Column<string>(type: "text", nullable: true),
                    LeaderCharacterId = table.Column<string>(type: "text", nullable: true),
                    MemberCount = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outfit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outfit_Faction_FactionId",
                        column: x => x.FactionId,
                        principalSchema: "public",
                        principalTable: "Faction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outfit_World_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "public",
                        principalTable: "World",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutfitMember",
                schema: "public",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    MemberSinceDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: false),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    RankOrdinal = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutfitMember", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_OutfitMember_Outfit_OutfitId",
                        column: x => x.OutfitId,
                        principalSchema: "public",
                        principalTable: "Outfit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Character",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BattleRank = table.Column<int>(type: "int4", nullable: false),
                    BattleRankPercentToNext = table.Column<int>(type: "int4", nullable: false),
                    CertsEarned = table.Column<int>(type: "int4", nullable: false),
                    FactionId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TitleId = table.Column<string>(type: "text", nullable: true),
                    WorldId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Faction_FactionId",
                        column: x => x.FactionId,
                        principalSchema: "public",
                        principalTable: "Faction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_CharacterTime_Id",
                        column: x => x.Id,
                        principalSchema: "public",
                        principalTable: "CharacterTime",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Character_OutfitMember_Id",
                        column: x => x.Id,
                        principalSchema: "public",
                        principalTable: "OutfitMember",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Character_Title_TitleId",
                        column: x => x.TitleId,
                        principalSchema: "public",
                        principalTable: "Title",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_World_WorldId",
                        column: x => x.WorldId,
                        principalSchema: "public",
                        principalTable: "World",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_MetagameEventId",
                schema: "public",
                table: "Alert",
                column: "MetagameEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_FactionId",
                schema: "public",
                table: "Character",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_TitleId",
                schema: "public",
                table: "Character",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_WorldId",
                schema: "public",
                table: "Character",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStat_Id",
                schema: "public",
                table: "CharacterStat",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStat_ProfileId",
                schema: "public",
                table: "CharacterStat",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatByFaction_Id",
                schema: "public",
                table: "CharacterStatByFaction",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatByFaction_ProfileId",
                schema: "public",
                table: "CharacterStatByFaction",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStat_Id",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStat_ItemId",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStat_Kills",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "Kills");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStat_VehicleId",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStatByFaction_Id",
                schema: "public",
                table: "CharacterWeaponStatByFaction",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStatByFaction_ItemId",
                schema: "public",
                table: "CharacterWeaponStatByFaction",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeaponStatByFaction_VehicleId",
                schema: "public",
                table: "CharacterWeaponStatByFaction",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_AttackerCharacterId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_AttackerOutfitId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerOutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_AttackerVehicleId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_AttackerWeaponId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerWeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_CharacterId",
                schema: "public",
                table: "EventDeath",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDeath_CharacterOutfitId",
                schema: "public",
                table: "EventDeath",
                column: "CharacterOutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemCategoryId",
                schema: "public",
                table: "Item",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfit_FactionId",
                schema: "public",
                table: "Outfit",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfit_LeaderCharacterId",
                schema: "public",
                table: "Outfit",
                column: "LeaderCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfit_WorldId",
                schema: "public",
                table: "Outfit",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitMember_OutfitId",
                schema: "public",
                table: "OutfitMember",
                column: "OutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSession_CharacterId_LoginDate_LogoutDate",
                schema: "public",
                table: "PlayerSession",
                columns: new[] { "CharacterId", "LoginDate", "LogoutDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFaction_DbVehicleId",
                schema: "public",
                table: "VehicleFaction",
                column: "DbVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStat_Character_CharacterId",
                schema: "public",
                table: "CharacterStat",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStat_Character_Id",
                schema: "public",
                table: "CharacterStat",
                column: "Id",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStatByFaction_Character_CharacterId",
                schema: "public",
                table: "CharacterStatByFaction",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStatByFaction_Character_Id",
                schema: "public",
                table: "CharacterStatByFaction",
                column: "Id",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterTime_Character_CharacterId",
                schema: "public",
                table: "CharacterTime",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterWeaponStat_Character_CharacterId",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterWeaponStat_Character_Id",
                schema: "public",
                table: "CharacterWeaponStat",
                column: "Id",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterWeaponStatByFaction_Character_CharacterId",
                schema: "public",
                table: "CharacterWeaponStatByFaction",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterWeaponStatByFaction_Character_Id",
                schema: "public",
                table: "CharacterWeaponStatByFaction",
                column: "Id",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDeath_Character_AttackerCharacterId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerCharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDeath_Character_CharacterId",
                schema: "public",
                table: "EventDeath",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDeath_Outfit_AttackerOutfitId",
                schema: "public",
                table: "EventDeath",
                column: "AttackerOutfitId",
                principalSchema: "public",
                principalTable: "Outfit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventDeath_Outfit_CharacterOutfitId",
                schema: "public",
                table: "EventDeath",
                column: "CharacterOutfitId",
                principalSchema: "public",
                principalTable: "Outfit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Outfit_Character_LeaderCharacterId",
                schema: "public",
                table: "Outfit",
                column: "LeaderCharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutfitMember_Character_CharacterId",
                schema: "public",
                table: "OutfitMember",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Character_Faction_FactionId",
                schema: "public",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_Outfit_Faction_FactionId",
                schema: "public",
                table: "Outfit");

            migrationBuilder.DropForeignKey(
                name: "FK_Character_CharacterTime_Id",
                schema: "public",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_Character_OutfitMember_Id",
                schema: "public",
                table: "Character");

            migrationBuilder.DropTable(
                name: "Alert",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterStat",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterStatByFaction",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterUpdateQueue",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterWeaponStat",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterWeaponStatByFaction",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventAchievementEarned",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventBattlerankUp",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventContinentLock",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventContinentUnkock",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventDeath",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventFacilityControl",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventGainExperience",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventMetagameEvent",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventPlayerFacilityCapture",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventPlayerFacilityDefend",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventPlayerLogin",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventPlayerLogout",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventVehicleDestroy",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FacilityLink",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MapHex",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MapRegion",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MetagameEventState",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PlayerSession",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VehicleFaction",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Zone",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MetagameEventCategory",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Profile",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Vehicle",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ItemCategory",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Faction",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CharacterTime",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OutfitMember",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Outfit",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Character",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Title",
                schema: "public");

            migrationBuilder.DropTable(
                name: "World",
                schema: "public");
        }
    }
}
