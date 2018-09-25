using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class FunctionalRepository : IFunctionalRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public FunctionalRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<CharacterLastSession>> GetPSBLastOnline()
        {
            var result = new List<CharacterLastSession>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Database.GetDbConnection().OpenAsync();

                using (var dbCmd = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    dbCmd.CommandText = "SELECT * FROM psb_last_online()";
                    var reader = await dbCmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new CharacterLastSession
                            {
                                CharacterId = reader.GetString(0),
                                Name = reader.GetString(1),
                                SessionId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                LoginDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                LogoutDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Duration = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                            };
                            result.Add(row);
                        }
                    }
                }

                return result;
            }
        }
    }
}
