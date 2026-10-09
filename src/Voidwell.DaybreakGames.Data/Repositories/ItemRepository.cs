using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public ItemRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Item>> FindItemsByIdsAsync(IEnumerable<int> itemIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Items.Where(i => itemIds.Contains(i.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> FindWeaponsByNameAsync(string name, int limit)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Items.Where(i => i.Name!.ToLower().Contains(name.ToLower()))
            .Where(a => a.ItemCategoryId < 99 || a.ItemCategoryId > 108 && a.ItemCategoryId < 133 || a.ItemCategoryId == 138 || a.ItemCategoryId == 144 || a.ItemCategoryId == 147 || a.ItemCategoryId == 157)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Items.Where(i => i.ItemCategoryId.HasValue && categoryIds.Contains(i.ItemCategoryId.Value))
            .ToListAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<Item> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }

    public async Task UpsertRangeAsync(IEnumerable<ItemCategory> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
