using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class SanctionedWeaponsRepository : ISanctionedWeaponsRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public SanctionedWeaponsRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<SanctionedWeapon>> GetAllSanctionedWeapons()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.SanctionedWeapons
                    .Where(a => a.Type == "i")
                    .ToListAsync();
            }
        }
    }
}
