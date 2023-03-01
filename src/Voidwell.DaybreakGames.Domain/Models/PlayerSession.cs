using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class PlayerSession
    {
        public IEnumerable<PlayerSessionEvent> Events { get; set; }
        public PlayerSessionInfo Session { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PlayerSessionEventType
    {
        Death = 0,
        FacilityCapture = 1,
        FacilityDefend = 2,
        BattleRankUp = 3,
        VehicleDestroy = 4,
        Login = 5,
        Logout = 6
    }

    [JsonConverter(typeof(PlayerSessionEventCreationConverter))]
    public abstract class PlayerSessionEvent
    {
        public DateTime Timestamp { get; set; }
        public int? ZoneId { get; set; }
        public abstract PlayerSessionEventType EventType { get; }
    }

    public class PlayerSessionLoginEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.Login;
    }

    public class PlayerSessionLogoutEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.Logout;
    }

    public class PlayerSessionDeathEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.Death;

        public CombatReportItemDetail Attacker { get; set; }
        public CombatReportItemDetail Victim { get; set; }
        public PlayerSessionWeapon Weapon { get; set; }
        public int? AttackerFireModeId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public string AttackerOutfitId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? CharacterLoadoutId { get; set; }
        public string CharacterOutfitId { get; set; }
        public bool IsHeadshot { get; set; }
    }

    public class PlayerSessionFacilityCaptureEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.FacilityCapture;

        public PlayerSessionFacility Facility { get; set; }
    }

    public class PlayerSessionFacilityDefendEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.FacilityDefend;

        public PlayerSessionFacility Facility { get; set; }
    }

    public class PlayerSessionBattleRankUpEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.BattleRankUp;

        public int BattleRank { get; set; }
    }

    public class PlayerSessionVehicleDestroyEvent : PlayerSessionEvent
    {
        public override PlayerSessionEventType EventType => PlayerSessionEventType.VehicleDestroy;

        public CombatReportItemDetail Attacker { get; set; }
        public CombatReportItemDetail Victim { get; set; }
        public PlayerSessionWeapon Weapon { get; set; }
        public PlayerSessionVehicle VictimVehicle { get; set; }
        public PlayerSessionFacility Facility { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? FactionId { get; set; }
    }

    public class PlayerSessionInfo
    {
        public string CharacterId { get; set; }
        public int Duration { get; set; } 
        public string Id { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
    }

    public class PlayerSessionWeapon
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string Name { get; set; }
    }

    public class PlayerSessionVehicle
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string Name { get; set; }
    }

    public class PlayerSessionFacility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class PlayerSessionEventCreationConverter : JsonConverter<PlayerSessionEvent>
    {
        protected static Type GetConvertType(JsonNode node)
        {
            var typeName = node["eventType"].ToString();
            var eventType = Enum.Parse(typeof(PlayerSessionEventType), typeName);
            return eventType switch
            {
                PlayerSessionEventType.Death => typeof(PlayerSessionDeathEvent),
                PlayerSessionEventType.BattleRankUp => typeof(PlayerSessionBattleRankUpEvent),
                PlayerSessionEventType.FacilityCapture => typeof(PlayerSessionFacilityCaptureEvent),
                PlayerSessionEventType.FacilityDefend => typeof(PlayerSessionFacilityDefendEvent),
                PlayerSessionEventType.VehicleDestroy => typeof(PlayerSessionVehicleDestroyEvent),
                PlayerSessionEventType.Login => typeof(PlayerSessionLoginEvent),
                PlayerSessionEventType.Logout => typeof(PlayerSessionLogoutEvent),
                _ => null,
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(PlayerSessionEvent).IsAssignableFrom(objectType);
        }

        public override PlayerSessionEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonNode = JsonNode.Parse(ref reader, new JsonNodeOptions { PropertyNameCaseInsensitive = true });

            var eventType = GetConvertType(jsonNode);

            return jsonNode.Deserialize(eventType, options) as PlayerSessionEvent;
        }
        
        public override void Write(Utf8JsonWriter writer, PlayerSessionEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
