using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface IDirectiveService
{
    Task<IEnumerable<DirectiveTreeCategory>?> GetDirectiveDataAsync();
}
