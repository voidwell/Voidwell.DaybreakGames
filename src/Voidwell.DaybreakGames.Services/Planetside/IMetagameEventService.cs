using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMetagameEventService : IUpdateable
    {
        Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId);
    }
}
