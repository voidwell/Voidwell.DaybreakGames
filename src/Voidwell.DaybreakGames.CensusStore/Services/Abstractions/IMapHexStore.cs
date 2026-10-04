using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface IMapHexStore
{
    Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId);
}
