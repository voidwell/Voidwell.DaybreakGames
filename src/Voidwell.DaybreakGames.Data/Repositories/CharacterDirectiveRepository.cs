using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class CharacterDirectiveRepository : ICharacterDirectiveRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public CharacterDirectiveRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<CharacterDirectiveTree>> GetDirectiveTreesAsync(string characterId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.CharacterDirectiveTrees.Where(a => a.CharacterId == characterId).ToListAsync();
    }

    public async Task<IEnumerable<CharacterDirectiveTier>> GetDirectiveTiersAsync(string characterId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.CharacterDirectiveTiers.Where(a => a.CharacterId == characterId).ToListAsync();
    }

    public async Task<IEnumerable<CharacterDirective>> GetDirectivesAsync(string characterId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var query = from charDirective in dbContext.CharacterDirectives
                    join directive in dbContext.Directives on charDirective.DirectiveId equals directive.Id
                    where charDirective.CharacterId == characterId
                    select new CharacterDirective
                    {
                        CharacterId = charDirective.CharacterId,
                        DirectiveId = charDirective.DirectiveId,
                        CompletionTimeDate = charDirective.CompletionTimeDate,
                        DirectiveTreeId = charDirective.DirectiveTreeId,
                        Directive = directive
                    };
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<CharacterDirectiveObjective>> GetDirectiveObjectivesAsync(string characterId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.CharacterDirectiveObjectives.Where(a => a.CharacterId == characterId).ToListAsync();
    }
}
