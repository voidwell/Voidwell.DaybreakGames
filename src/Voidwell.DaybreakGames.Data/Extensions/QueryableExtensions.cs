using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Keeps characters on the given worlds. A null list keeps every character.
    /// </summary>
    public static IQueryable<Character> WhereWorldIn(this IQueryable<Character> query, IEnumerable<int>? worldIds)
    {
        if (worldIds == null)
        {
            return query;
        }

        var ids = worldIds.ToList();
        return query.Where(a => ids.Contains(a.WorldId));
    }

    /// <summary>
    /// Keeps outfits on the given worlds. A null list keeps every outfit.
    /// </summary>
    public static IQueryable<Outfit> WhereWorldIn(this IQueryable<Outfit> query, IEnumerable<int>? worldIds)
    {
        if (worldIds == null)
        {
            return query;
        }

        var ids = worldIds.ToList();
        return query.Where(a => a.WorldId != null && ids.Contains(a.WorldId.Value));
    }
}
