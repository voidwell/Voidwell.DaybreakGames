using static Voidwell.DaybreakGames.Data.DbContextHelper;

namespace Voidwell.DaybreakGames.Data
{
    public interface IDbContextHelper
    {
        PS2DbContext Create();
        DbContextFactory GetFactory();
    }
}
