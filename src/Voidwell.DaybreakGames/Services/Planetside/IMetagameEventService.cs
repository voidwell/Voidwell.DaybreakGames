using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMetagameEventService : IUpdateable
    {
        Task RefreshStore();
    }
}
