using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ITitleService : IUpdateable
    {
        Task RefreshStore();
    }
}
