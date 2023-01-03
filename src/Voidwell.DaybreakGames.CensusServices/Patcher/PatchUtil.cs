using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.CensusServices.Patcher
{
    public static class PatchUtil
    {
        public static IEnumerable<T> PatchData<T>(Func<T, object> key, IEnumerable<T> target, IEnumerable<T> source)
        {
            if (target == null || !target.Any())
            {
                return source;
            }
            else if (source == null || !source.Any())
            {
                return target;
            }

            var patchMap = new Dictionary<object, T>();
            target.ToList().ForEach(x => patchMap.Add(key(x), x));
            source.ToList().ForEach(x => patchMap.TryAdd(key(x), x));

            return patchMap.Values.ToList();
        }
    }
}
