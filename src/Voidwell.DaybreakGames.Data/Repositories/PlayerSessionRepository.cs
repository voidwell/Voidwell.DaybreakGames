using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class PlayerSessionRepository : IPlayerSessionRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public PlayerSessionRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task AddAsync(DbPlayerSession entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                await dbContext.PlayerSessions.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<DbPlayerSession> GetPlayerSessionAsync(string sessionId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.PlayerSessions.FirstOrDefaultAsync(a => a.Id.ToString() == sessionId);
            }
        }

        public async Task<IEnumerable<DbPlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.PlayerSessions.Where(a => a.CharacterId == characterId && a.LogoutDate != null)
                    .OrderBy("LoginDate", SortDirection.Descending)
                    .Take(limit)
                    .ToArrayAsync();
            }
        }
    }
}
