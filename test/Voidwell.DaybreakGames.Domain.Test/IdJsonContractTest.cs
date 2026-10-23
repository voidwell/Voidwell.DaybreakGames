using System.Text.Json;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Utils.JsonConverters;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

// Character and outfit ids are 19 digits, past what JavaScript numbers hold exactly,
// so they stay strings in JSON even though they are longs in code.
public class IdJsonContractTest
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new LongJsonConverter() }
    };

    private const long _characterId = 5428010618035323201;
    private const long _outfitId = 37509488620604883;

    [Fact]
    public void DomainIds_SerializeAsStrings()
    {
        var details = new CharacterDetails
        {
            Id = _characterId,
            Outfit = new CharacterDetailsOutfit { Id = _outfitId }
        };

        var json = JsonSerializer.Serialize(details, _serializerOptions);

        Assert.Contains("\"Id\":\"5428010618035323201\"", json);
        Assert.Contains("\"Id\":\"37509488620604883\"", json);
    }

    [Fact]
    public void DataIds_ReadFromCachedStringValues()
    {
        // Entries cached before ids became longs hold them as strings
        var json = "{\"CharacterId\":\"5428010618035323201\",\"OutfitId\":\"37509488620604883\",\"Rank\":\"Member\"}";

        var member = JsonSerializer.Deserialize<OutfitMember>(json, _serializerOptions);

        Assert.Equal(_characterId, member!.CharacterId);
        Assert.Equal(_outfitId, member.OutfitId);
    }

    [Fact]
    public void DataIds_RoundTripWithoutLosingPrecision()
    {
        var death = new Data.Models.Planetside.Events.Death { CharacterId = _characterId, AttackerCharacterId = _characterId - 1, AttackerOutfitId = _outfitId };

        var result = JsonSerializer.Deserialize<Data.Models.Planetside.Events.Death>(JsonSerializer.Serialize(death, _serializerOptions), _serializerOptions);

        Assert.Equal(_characterId, result!.CharacterId);
        Assert.Equal(_characterId - 1, result.AttackerCharacterId);
        Assert.Equal(_outfitId, result.AttackerOutfitId);
        Assert.Null(result.CharacterOutfitId);
    }
}
