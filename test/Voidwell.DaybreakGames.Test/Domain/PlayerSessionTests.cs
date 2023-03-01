using FluentAssertions;
using System;
using System.Text.Json;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Test.Domain
{
    public class PlayerSessionTests
    {
        [Fact]
        public void Serialize_ObjectToJson()
        {
            // Arrange
            var playerSession = new PlayerSession
            {
                Session = new PlayerSessionInfo
                {
                    Id = "4321",
                    CharacterId = "1234",
                    Duration = 10,
                    LoginDate = new DateTime(2023, 1, 30, 2, 3, 4),
                    LogoutDate = new DateTime(2023, 1, 30, 4, 3, 4)
                },
                Events = new PlayerSessionEvent[]
                {
                    new PlayerSessionLoginEvent
                    {
                        Timestamp = new DateTime(2023, 1, 30, 2, 3, 4),
                        ZoneId = 4
                    },
                    new PlayerSessionBattleRankUpEvent
                    {
                        BattleRank = 10,
                        Timestamp = new DateTime(2023, 1, 30, 3, 3, 4),
                        ZoneId = 2
                    }
                }
            };

            // Act
            var result = JsonSerializer.Serialize(playerSession);

            // Assert
            result.Should()
                .Be("{\"Events\":[{\"EventType\":\"Login\",\"Timestamp\":\"2023-01-30T02:03:04\",\"ZoneId\":4},{\"EventType\":\"BattleRankUp\",\"BattleRank\":10,\"Timestamp\":\"2023-01-30T03:03:04\",\"ZoneId\":2}],\"Session\":{\"CharacterId\":\"1234\",\"Duration\":10,\"Id\":\"4321\",\"LoginDate\":\"2023-01-30T02:03:04\",\"LogoutDate\":\"2023-01-30T04:03:04\"}}");
        }

        [Fact]
        public void Deserialize_JsonToObject()
        {
            // Arrange
            var playerSession = new PlayerSession
            {
                Session = new PlayerSessionInfo
                {
                    Id = "4321",
                    CharacterId = "1234",
                    Duration = 10,
                    LoginDate = new DateTime(2023, 1, 30, 2, 3, 4),
                    LogoutDate = new DateTime(2023, 1, 30, 4, 3, 4)
                },
                Events = new PlayerSessionEvent[]
                {
                    new PlayerSessionLoginEvent
                    {
                        Timestamp = new DateTime(2023, 1, 30, 2, 3, 4),
                        ZoneId = 4
                    },
                    new PlayerSessionBattleRankUpEvent
                    {
                        BattleRank = 10,
                        Timestamp = new DateTime(2023, 1, 30, 3, 3, 4),
                        ZoneId = 2
                    }
                }
            };
            var json = "{\"Events\":[{\"EventType\":\"Login\",\"Timestamp\":\"2023-01-30T02:03:04\",\"ZoneId\":4},{\"EventType\":\"BattleRankUp\",\"BattleRank\":10,\"Timestamp\":\"2023-01-30T03:03:04\",\"ZoneId\":2}],\"Session\":{\"CharacterId\":\"1234\",\"Duration\":10,\"Id\":\"4321\",\"LoginDate\":\"2023-01-30T02:03:04\",\"LogoutDate\":\"2023-01-30T04:03:04\"}}";

            // Act
            var result = JsonSerializer.Deserialize<PlayerSession>(json);

            // Assert
            result.Should()
                .BeEquivalentTo(playerSession);
        }
    }
}
