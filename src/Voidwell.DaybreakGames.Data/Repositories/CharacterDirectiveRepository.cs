using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class CharacterDirectiveRepository : ICharacterDirectiveRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterDirectiveRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<CharacterDirectiveTree>> GetDirectiveTreesAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterDirectiveTrees.Where(a => a.CharacterId == characterId).ToListAsync();
            }
        }

        public async Task<IEnumerable<CharacterDirectiveTier>> GetDirectiveTiersAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterDirectiveTiers.Where(a => a.CharacterId == characterId).ToListAsync();
            }
        }

        public async Task<IEnumerable<CharacterDirective>> GetDirectivesAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

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
        }

        public async Task<IEnumerable<CharacterDirectiveObjective>> GetDirectiveObjectivesAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.CharacterDirectiveObjectives.Where(a => a.CharacterId == characterId).ToListAsync();
            }
        }
    }
}
