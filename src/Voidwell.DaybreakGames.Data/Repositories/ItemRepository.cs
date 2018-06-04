using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ItemRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Item>> FindItemsByIdsAsync(IEnumerable<int> itemIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Items.Where(i => itemIds.Contains(i.Id))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Item>> FindItemsByNameAsync(string name, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Items.Where(i => i.Name.ToLower().Contains(name.ToLower()))
                    .Where(a => a.ItemCategoryId < 99 )
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<Item> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Items.UpsertRangeAsync(entities, a => entities.Any(e => e.Id == a.Id), (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<ItemCategory> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.ItemCategories.UpsertRangeAsync(entities, a => entities.Any(e => e.Id == a.Id), (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
