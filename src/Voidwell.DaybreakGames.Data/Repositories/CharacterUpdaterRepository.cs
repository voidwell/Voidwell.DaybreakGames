using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterUpdaterRepository : ICharacterUpdaterRepository
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<ICharacterUpdaterRepository> _logger;

        public CharacterUpdaterRepository(IDbContextHelper dbContextHelper, ILogger<ICharacterUpdaterRepository> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        public async Task AddAsync(CharacterUpdateQueue entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storeEntity = await dbContext.CharacterUpdateQueue.FirstOrDefaultAsync(a => a.CharacterId == entity.CharacterId);
                if (storeEntity == null)
                {
                    _logger.LogInformation($"Adding character ${entity.CharacterId}");
                    dbContext.CharacterUpdateQueue.Add(entity);
                }
                else
                {
                    _logger.LogInformation($"Updating character ${storeEntity.CharacterId}");
                    storeEntity.Timestamp = DateTime.UtcNow;
                    dbContext.CharacterUpdateQueue.Update(storeEntity);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(CharacterUpdateQueue entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                dbContext.CharacterUpdateQueue.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync(TimeSpan? delay = null)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterUpdateQueue
                    .Where(a => DateTime.UtcNow - a.Timestamp >= delay)
                    .OrderBy(a => a.Timestamp)
                    .ToListAsync();
            }
        }

        public async Task<int> GetQueueLengthAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterUpdateQueue
                    .CountAsync();
            }
        }
    }
}
