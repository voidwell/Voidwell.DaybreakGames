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
    }
}
