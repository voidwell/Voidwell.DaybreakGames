using static Voidwell.DaybreakGames.Data.DbContextHelper;

namespace Voidwell.DaybreakGames.Data
{
    public interface IDbContextHelper
    {
        DbContextFactory GetFactory();
    }
}
