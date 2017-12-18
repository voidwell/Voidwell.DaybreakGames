using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Data
{
    public class DbContextHelper : IDbContextHelper
    {
        private readonly DbContextOptions<PS2DbContext> _options;

        public DbContextHelper(DbContextOptions<PS2DbContext> options)
        {
            _options = options;
        }

        public PS2DbContext Create()
        {
            return new PS2DbContext(_options);
        }
    }
}
