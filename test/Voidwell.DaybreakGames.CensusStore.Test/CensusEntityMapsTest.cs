using Voidwell.DaybreakGames.Census.Models;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class CensusEntityMapsTest
{
    [Fact]
    public void Faction_MapsIdAndEnglishName()
    {
        var model = new CensusFactionModel
        {
            FactionId = 3,
            Name = new MultiLanguageString { English = "Terran Republic" },
            ImageId = 12,
            CodeTag = "TR",
            UserSelectable = true
        };

        var faction = CensusEntityMaps.Map(model);

        Assert.Equal(3, faction.Id);
        Assert.Equal("Terran Republic", faction.Name);
        Assert.Equal(12, faction.ImageId);
        Assert.Equal("TR", faction.CodeTag);
        Assert.True(faction.UserSelectable);
    }

    [Fact]
    public void World_MapsIdAndEnglishName()
    {
        var world = CensusEntityMaps.Map(new CensusWorldModel
        {
            WorldId = 17,
            State = "online",
            Name = new MultiLanguageString { English = "Emerald" }
        });

        Assert.Equal(17, world.Id);
        Assert.Equal("Emerald", world.Name);
    }

    [Fact]
    public void CharacterAchievement_RealFinishDate_IsMapped()
    {
        var finished = new DateTime(2023, 5, 17, 0, 0, 0, DateTimeKind.Utc);

        var result = CensusEntityMaps.Map(new CensusCharacterAchievementModel
        {
            CharacterId = "1",
            AchievementId = 5,
            FinishDate = finished
        });

        Assert.Equal(1, result.CharacterId);
        Assert.Equal(5, result.AchievementId);
        Assert.Equal(finished, result.FinishDate);
    }

    [Fact]
    public void CharacterAchievement_EpochFinishDate_IsTreatedAsNotFinished()
    {
        var result = CensusEntityMaps.Map(new CensusCharacterAchievementModel
        {
            CharacterId = "1",
            AchievementId = 5,
            FinishDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Null(result.FinishDate);
    }

    [Fact]
    public void CharacterDirective_EpochCompletionDate_IsTreatedAsNotCompleted()
    {
        var result = CensusEntityMaps.Map(new CensusCharacterDirectiveModel
        {
            CharacterId = "1",
            DirectiveId = 9,
            CompletionTimeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Null(result.CompletionTimeDate);
    }

    [Fact]
    public void CharacterDirective_RealCompletionDate_IsMapped()
    {
        var completed = new DateTime(2022, 2, 2, 0, 0, 0, DateTimeKind.Utc);

        var result = CensusEntityMaps.Map(new CensusCharacterDirectiveModel
        {
            CharacterId = "1",
            DirectiveId = 9,
            CompletionTimeDate = completed
        });

        Assert.Equal(completed, result.CompletionTimeDate);
        Assert.Equal(9, result.DirectiveId);
    }
}
