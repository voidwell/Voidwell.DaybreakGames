using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public interface ICensusStaticCollection<T> : ICensusCollection where T : class
    {
        Task<IEnumerable<T>> GetCollectionAsync();
    }
}
