using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public EventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<DbEventDeath>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.EventDeaths.Where(a => a.AttackerCharacterId == characterId || a.CharacterId == characterId && a.Timestamp > lower && a.Timestamp < upper)
                    .Include(i => i.AttackerCharacter)
                    .Include(i => i.Character)
                    .Include(i => i.AttackerWeapon)
                    .ToArrayAsync();
            }
        }
    }
}
