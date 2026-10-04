namespace Voidwell.DaybreakGames.Api.Authentication;

public interface IUserRolesClient
{
    Task<IEnumerable<string>?> GetRolesAsync(Guid userId);
}
