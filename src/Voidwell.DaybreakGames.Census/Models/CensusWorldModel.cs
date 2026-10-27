using System.Text.Json.Serialization;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Models;

public class CensusWorldModel
{
    public int WorldId { get; set; }
    public string? State { get; set; }
    public MultiLanguageString? Name { get; set; }

    /// <summary>
    /// The platform whose namespace returned the world. Census doesn't send it.
    /// </summary>
    [JsonIgnore]
    public Platform Platform { get; set; }
}
