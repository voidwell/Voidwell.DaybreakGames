using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class DirectiveRepository : IDirectiveRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public DirectiveRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<DirectiveTreeCategory>> GetDirectiveTreesCategoriesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DirectiveTreeCategories.ToListAsync();
    }

    public async Task<IEnumerable<DirectiveTree>> GetDirectiveTreesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DirectiveTrees.ToListAsync();
    }

    public async Task<IEnumerable<DirectiveTier>> GetDirectiveTiersAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DirectiveTiers.ToListAsync();
    }

    public async Task<IEnumerable<Directive>> GetDirectivesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var query = from directive in dbContext.Directives
                    join objectiveSet in dbContext.ObjectiveSetsToObjective on directive.ObjectiveSetId equals objectiveSet.ObjectiveSetId into objectiveSets
                    from objectiveSet in objectiveSets.DefaultIfEmpty()

                    join imageSet in dbContext.ImageSets on new { imageSetId = directive.ImageSetId!.Value, typeId = 6 } equals new { imageSetId = imageSet.Id, typeId = imageSet.TypeId } into imageSets
                    from imageSet in imageSets.DefaultIfEmpty()

                    select new Directive
                    {
                        Id = directive.Id,
                        Name = directive.Name,
                        Description = directive.Description,
                        DirectiveTierId = directive.DirectiveTierId,
                        DirectiveTreeId = directive.DirectiveTreeId,
                        ImageId = directive.ImageId,
                        ImageSetId = directive.ImageSetId,
                        ObjectiveSetId = directive.ObjectiveSetId,
                        QualifyRequirementId = directive.QualifyRequirementId,
                        ObjectiveSet = objectiveSet == null ? null : new ObjectiveSetToObjective
                        {
                            ObjectiveSetId = objectiveSet.ObjectiveSetId,
                            ObjectiveGroupId = objectiveSet.ObjectiveGroupId
                        },
                        ImageSet = imageSet
                    };

        return await query.ToListAsync();
    }
}
