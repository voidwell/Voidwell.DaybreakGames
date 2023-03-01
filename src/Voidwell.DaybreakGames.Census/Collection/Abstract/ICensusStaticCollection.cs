using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public interface ICensusStaticCollection
    {
    }

    public interface ICensusStaticCollection<TCensusType> : ICensusStaticCollection, ICensusCollection<TCensusType> where TCensusType : class
    {
        Task<IEnumerable<TCensusType>> GetCollectionAsync();
    }
}
