using System;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services
{
    public interface IUpdateable
    {
       string ServiceName { get; }
       TimeSpan UpdateInterval { get; }
       Task RefreshStore();
    }
}
