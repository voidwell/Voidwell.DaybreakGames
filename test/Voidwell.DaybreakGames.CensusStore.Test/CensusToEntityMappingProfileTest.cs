using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class CensusToEntityMappingProfileTest
{
    private static readonly MapperConfiguration _configuration =
        new(cfg => cfg.AddProfile<CensusToEntityMappingProfile>(), NullLoggerFactory.Instance);

    private readonly IMapper _mapper = _configuration.CreateMapper();

    [Fact]
    public void Configuration_AllMapsCompile()
    {
        var act = () => _configuration.CompileMappings();

        act.Should().NotThrow();
    }

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

        var faction = _mapper.Map<Faction>(model);

        faction.Id.Should().Be(3);
        faction.Name.Should().Be("Terran Republic");
        faction.ImageId.Should().Be(12);
        faction.CodeTag.Should().Be("TR");
        faction.UserSelectable.Should().BeTrue();
    }

    [Fact]
    public void World_MapsIdAndEnglishName()
    {
        var world = _mapper.Map<World>(new CensusWorldModel
        {
            WorldId = 17,
            State = "online",
            Name = new MultiLanguageString { English = "Emerald" }
        });

        world.Id.Should().Be(17);
        world.Name.Should().Be("Emerald");
    }

    [Fact]
    public void CharacterAchievement_RealFinishDate_IsMapped()
    {
        var finished = new DateTime(2023, 5, 17, 0, 0, 0, DateTimeKind.Utc);

        var result = _mapper.Map<CharacterAchievement>(new CensusCharacterAchievementModel
        {
            CharacterId = "c1",
            AchievementId = 5,
            FinishDate = finished
        });

        result.CharacterId.Should().Be("c1");
        result.AchievementId.Should().Be(5);
        result.FinishDate.Should().Be(finished);
    }

    [Fact]
    public void CharacterAchievement_EpochFinishDate_IsTreatedAsNotFinished()
    {
        var result = _mapper.Map<CharacterAchievement>(new CensusCharacterAchievementModel
        {
            CharacterId = "c1",
            AchievementId = 5,
            FinishDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        result.FinishDate.Should().BeNull();
    }

    [Fact]
    public void CharacterDirective_EpochCompletionDate_IsTreatedAsNotCompleted()
    {
        var result = _mapper.Map<CharacterDirective>(new CensusCharacterDirectiveModel
        {
            CharacterId = "c1",
            DirectiveId = 9,
            CompletionTimeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        result.CompletionTimeDate.Should().BeNull();
    }

    [Fact]
    public void CharacterDirective_RealCompletionDate_IsMapped()
    {
        var completed = new DateTime(2022, 2, 2, 0, 0, 0, DateTimeKind.Utc);

        var result = _mapper.Map<CharacterDirective>(new CensusCharacterDirectiveModel
        {
            CharacterId = "c1",
            DirectiveId = 9,
            CompletionTimeDate = completed
        });

        result.CompletionTimeDate.Should().Be(completed);
        result.DirectiveId.Should().Be(9);
    }
}
