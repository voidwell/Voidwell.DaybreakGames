using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public class DbContextBase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql("");
        }
    }
}
