using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IFactionService
    {
        Task RefreshStore();
    }
}