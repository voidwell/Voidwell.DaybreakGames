using System;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStream
{
    public interface IEventValidator
    {
        Task<bool> Validiate<T>(T ev, Func<T, string> keyExpr, Func<T, bool> cleanupExpr) where T : class;
    }
}