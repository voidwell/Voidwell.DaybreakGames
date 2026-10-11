using System.Collections;
using System.Reflection;

namespace Voidwell.DaybreakGames.CensusStore;

public interface ICensusToEntityMapper
{
    /// <summary>
    /// Maps each census model to its database entity.
    /// </summary>
    IEnumerable<TEntity> Map<TModel, TEntity>(IEnumerable<TModel> models);

    /// <summary>
    /// Maps a collection whose model and entity types are only known at runtime.
    /// Returns a <see cref="List{T}"/> of <paramref name="entityType"/>.
    /// </summary>
    object Map(IEnumerable models, Type modelType, Type entityType);
}

public class CensusToEntityMapper : ICensusToEntityMapper
{
    private static readonly Dictionary<(Type Model, Type Entity), MethodInfo> _maps = typeof(CensusEntityMaps)
        .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Where(a => a.GetParameters().Length == 1)
        .ToDictionary(a => (a.GetParameters()[0].ParameterType, a.ReturnType));

    public IEnumerable<TEntity> Map<TModel, TEntity>(IEnumerable<TModel> models)
    {
        return (List<TEntity>)Map(models, typeof(TModel), typeof(TEntity));
    }

    public object Map(IEnumerable models, Type modelType, Type entityType)
    {
        if (!_maps.TryGetValue((modelType, entityType), out var map))
        {
            throw new InvalidOperationException($"No census mapping is defined from '{modelType.Name}' to '{entityType.Name}'.");
        }

        var entities = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(entityType))!;
        var args = new object?[1];

        foreach (var model in models)
        {
            args[0] = model;
            entities.Add(map.Invoke(null, args));
        }

        return entities;
    }
}
