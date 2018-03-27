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

        public async Task<IEnumerable<WeaponAggregate>> GetWeaponAggregates()
        {
            var result = new List<WeaponAggregate>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Database.GetDbConnection().OpenAsync();

                using (var dbCmd = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    dbCmd.CommandText = "SELECT * FROM weapon_aggregate()";
                    var reader = await dbCmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new WeaponAggregate
                            {
                                ItemId = reader.GetInt32(0),
                                VehicleId = reader.GetInt32(1),
                                AVGKills = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                                STDKills = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                                AVGDeaths = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
                                STDDeaths = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                                AVGFireCount = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                                STDFireCount = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
                                AVGHitCount = reader.IsDBNull(8) ? 0 : reader.GetDouble(8),
                                STDHitCount = reader.IsDBNull(9) ? 0 : reader.GetDouble(9),
                                AVGHeadshots = reader.IsDBNull(10) ? 0 : reader.GetDouble(10),
                                STDHeadshots = reader.IsDBNull(11) ? 0 : reader.GetDouble(11),
                                AVGPlayTime = reader.IsDBNull(12) ? 0 : reader.GetDouble(12),
                                STDPlayTime = reader.IsDBNull(13) ? 0 : reader.GetDouble(13),
                                AVGScore = reader.IsDBNull(14) ? 0 : reader.GetDouble(14),
                                STDScore = reader.IsDBNull(15) ? 0 : reader.GetDouble(15),
                                AVGVehicleKills = reader.IsDBNull(16) ? 0 : reader.GetDouble(16),
                                STDVehicleKills = reader.IsDBNull(17) ? 0 : reader.GetDouble(17),
                                AVGKdr = reader.IsDBNull(18) ? 0 : reader.GetDouble(18),
                                STDKdr = reader.IsDBNull(19) ? 0 : reader.GetDouble(19),
                                AVGAccuracy = reader.IsDBNull(20) ? 0 : reader.GetDouble(20),
                                STDAccuracy = reader.IsDBNull(21) ? 0 : reader.GetDouble(21),
                                AVGHsr = reader.IsDBNull(22) ? 0 : reader.GetDouble(22),
                                STDHsr = reader.IsDBNull(23) ? 0 : reader.GetDouble(23),
                                AVGKph = reader.IsDBNull(24) ? 0 : reader.GetDouble(24),
                                STDKph = reader.IsDBNull(25) ? 0 : reader.GetDouble(25),
                                AVGVkph = reader.IsDBNull(26) ? 0 : reader.GetDouble(26),
                                STDVkph = reader.IsDBNull(27) ? 0 : reader.GetDouble(27),
                            };
                            result.Add(row);
                        }
                    }
                }

                return result;
            }
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
