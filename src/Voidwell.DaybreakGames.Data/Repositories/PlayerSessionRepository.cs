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

        public async Task AddAsync(PlayerSession entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.PlayerSessions.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<PlayerSession> GetPlayerSessionAsync(int sessionId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.PlayerSessions.FirstOrDefaultAsync(a => a.Id == sessionId);
            }
        }

        public async Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.PlayerSessions.Where(a => a.CharacterId == characterId && a.LogoutDate != null)
                    .OrderBy("LoginDate", SortDirection.Descending)
                    .Take(limit)
                    .ToArrayAsync();
            }
        }
    }
}
