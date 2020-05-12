using System;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public interface IUpdatable
    {
       string ServiceName { get; }
       TimeSpan UpdateInterval { get; }
       Task UpdateAsync();
    }
}
