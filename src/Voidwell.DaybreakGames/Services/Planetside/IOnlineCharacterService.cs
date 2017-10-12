using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IOnlineCharacterService
    {
        Task SetCharacterOnline(string characterId, DateTime loginDate);
        Task SetCharacterOffline(string characterId, DateTime logoutDate);
        IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(string worldId);
    }
}
