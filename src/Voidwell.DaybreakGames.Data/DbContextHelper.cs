using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Voidwell.DaybreakGames.Data
{
    public class DbContextHelper : IDbContextHelper
    {
        private readonly DbContextOptions<PS2DbContext> _options;
        private readonly IServiceScopeFactory _scopeFactory;

        public DbContextHelper(DbContextOptions<PS2DbContext> options, IServiceScopeFactory scopeFactory)
        {
            _options = options;
            _scopeFactory = scopeFactory;
        }

        public PS2DbContext Create()
        {
            return new PS2DbContext(_options);
        }

        public DbContextFactory GetFactory()
        {
            return new DbContextFactory(_scopeFactory);
        }

        public class DbContextFactory : IDisposable
        {
            private readonly IServiceScope _scope;
            private readonly PS2DbContext _dbContext;

            public DbContextFactory(IServiceScopeFactory scopeFactory)
            {
                _scope = scopeFactory.CreateScope();
                _dbContext = _scope.ServiceProvider.GetRequiredService<PS2DbContext>();
            }

            public PS2DbContext GetDbContext()
            {
                return _dbContext;
            }

            public void Dispose()
            {
                _scope.Dispose();
            }
        }
    }
}
