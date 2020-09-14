using System;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStore
{
    public interface IUpdateable
    {
        string StoreName { get; }
        TimeSpan UpdateInterval { get; }
        Task RefreshStore();
    }
}
