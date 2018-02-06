using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterUpdaterRepository : ICharacterUpdaterRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterUpdaterRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task AddAsync(CharacterUpdateQueue entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                dbContext.CharacterUpdateQueue.Add(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(CharacterUpdateQueue entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                dbContext.CharacterUpdateQueue.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync()
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.CharacterUpdateQueue
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
