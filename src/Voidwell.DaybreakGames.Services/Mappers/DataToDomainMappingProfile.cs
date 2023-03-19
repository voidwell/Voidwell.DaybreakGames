using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Services.Mappers
{
    public class DataToDomainMappingProfile : Profile
    {
        public DataToDomainMappingProfile()
        {
            CreateMap<Data.Models.Planetside.Character, Domain.Models.CharacterDetails>()
                .ForMember(d => d.Faction, opt => opt.MapFrom(s => s.Faction.Name))
                .ForMember(d => d.FactionImageId, opt => opt.MapFrom(s => s.Faction.ImageId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title.Name))
                .ForMember(d => d.World, opt => opt.MapFrom(s => s.World.Name))
                .ForMember(d => d.Times, opt => opt.MapFrom(s => s.Time))
                .ForMember(d => d.LifetimeStats, opt => opt.MapFrom(s => s.LifetimeStats))
                .ForMember(d => d.ProfileStats, opt => opt.MapFrom(s => s.Stats))
                .ForMember(d => d.ProfileStatsByFaction, opt => opt.MapFrom(s => s.StatsByFaction))
                .ForMember(d => d.StatsHistory, opt => opt.MapFrom(s => s.StatsHistory))
                .ForMember(d => d.Outfit, opt => opt.MapFrom(s => s.OutfitMembership))
                .ForMember(d => d.VehicleStats, opt => opt.MapFrom<CharacterVehicleStatsResolver>());

            CreateMap<Data.Models.Planetside.CharacterTime, Domain.Models.CharacterDetailsTimes>();

            CreateMap<Data.Models.Planetside.CharacterLifetimeStat, Domain.Models.CharacterDetailsLifetimeStats>()
                .ForMember(d => d.Kills, opt => opt.MapFrom(s => s.WeaponKills))
                .ForMember(d => d.PlayTime, opt => opt.MapFrom(s => s.WeaponPlayTime))
                .ForMember(d => d.VehicleKills, opt => opt.MapFrom(s => s.WeaponVehicleKills))
                .ForMember(d => d.DamageGiven, opt => opt.MapFrom(s => s.WeaponDamageGiven))
                .ForMember(d => d.DamageTakenBy, opt => opt.MapFrom(s => s.WeaponDamageTakenBy))
                .ForMember(d => d.FireCount, opt => opt.MapFrom(s => s.WeaponFireCount))
                .ForMember(d => d.HitCount, opt => opt.MapFrom(s => s.WeaponHitCount))
                .ForMember(d => d.Score, opt => opt.MapFrom(s => s.WeaponScore))
                .ForMember(d => d.Deaths, opt => opt.MapFrom(s => s.WeaponDeaths))
                .ForMember(d => d.Headshots, opt => opt.MapFrom(s => s.WeaponHeadshots));

            CreateMap<Data.Models.Planetside.CharacterStat, Domain.Models.CharacterDetailsProfileStat>()
                .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.Profile.Name))
                .ForMember(d => d.ImageId, opt => opt.MapFrom(s => s.Profile.ImageId));

            CreateMap<Data.Models.Planetside.CharacterStatByFaction, Domain.Models.CharacterDetailsProfileStatByFaction>()
                .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.Profile.Name))
                .ForMember(d => d.ImageId, opt => opt.MapFrom(s => s.Profile.ImageId))
                .ForMember(d => d.Kills, opt => opt.MapFrom(s => new Domain.Models.CharacterDetailsProfileStatByFactionValue
                {
                    Vs = s.KillsVS.GetValueOrDefault(),
                    Nc = s.KillsNC.GetValueOrDefault(),
                    Tr = s.KillsTR.GetValueOrDefault()
                }))
                .ForMember(d => d.KilledBy, opt => opt.MapFrom(s => new Domain.Models.CharacterDetailsProfileStatByFactionValue
                {
                    Vs = s.KilledByVS.GetValueOrDefault(),
                    Nc = s.KilledByNC.GetValueOrDefault(),
                    Tr = s.KilledByTR.GetValueOrDefault()
                }));

            CreateMap<Data.Models.Planetside.CharacterStatHistory, Domain.Models.CharacterDetailsStatsHistory>()
                .ForMember(d => d.Day, opt => opt.MapFrom(s => JToken.Parse(s.Day).ToObject<IEnumerable<int>>()))
                .ForMember(d => d.Week, opt => opt.MapFrom(s => JToken.Parse(s.Week).ToObject<IEnumerable<int>>()))
                .ForMember(d => d.Month, opt => opt.MapFrom(s => JToken.Parse(s.Month).ToObject<IEnumerable<int>>()));

            CreateMap<Data.Models.Planetside.OutfitMember, Domain.Models.CharacterDetailsOutfit>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.OutfitId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Outfit.Name))
                .ForMember(d => d.Alias, opt => opt.MapFrom(s => s.Outfit.Alias))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.Outfit.CreatedDate))
                .ForMember(d => d.MemberCount, opt => opt.MapFrom(s => s.Outfit.MemberCount));

            CreateMap<Data.Models.Planetside.CharacterWeaponStat, Domain.Models.CharacterDetailsWeaponStat>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Item.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Item.ItemCategory.Name))
                .ForMember(d => d.ImageId, opt => opt.MapFrom(s => s.Item.ImageId))
                .ForMember(d => d.VehicleName, opt => opt.MapFrom(s => s.Vehicle.Name))
                .ForMember(d => d.VehicleImageId, opt => opt.MapFrom(s => s.Vehicle.ImageId))
                .ForMember(d => d.Stats, opt => opt.MapFrom(s => s));

            CreateMap<Data.Models.Planetside.CharacterWeaponStat, Domain.Models.CharacterDetailsWeaponStatValue>();


            CreateMap<Data.Models.Planetside.DirectiveTreeCategory, Domain.Models.CharacterDirectivesOutlineCategory>()
                .ForMember(a => a.Trees, opt => opt.Ignore());

            CreateMap<Data.Models.Planetside.DirectiveTree, Domain.Models.CharacterDirectivesOutlineTree>()
                .ForMember(a => a.Tiers, opt => opt.Ignore());

            CreateMap<Data.Models.Planetside.DirectiveTier, Domain.Models.CharacterDirectivesOutlineTier>()
                .ForMember(d => d.TierId, opt => opt.MapFrom(s => s.DirectiveTierId))
                .ForMember(d => d.TreeId, opt => opt.MapFrom(s => s.DirectiveTreeId))
                .ForMember(a => a.Directives, opt => opt.Ignore());

            CreateMap<Data.Models.Planetside.RewardGroupToReward, Domain.Models.DirectivesOutlineReward>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RewardId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Reward.Item.Name))
                .ForMember(d => d.ImageId, opt => opt.MapFrom(s => s.Reward.Item.ImageId));

            CreateMap<Data.Models.Planetside.Directive, Domain.Models.CharacterDirectivesOutlineDirective>()
                .ForMember(d => d.TierId, opt => opt.MapFrom(s => s.DirectiveTierId))
                .ForMember(d => d.TreeId, opt => opt.MapFrom(s => s.DirectiveTreeId))
                .ForMember(d => d.ImageId, opt => opt.MapFrom(s => s.ImageSet != null ? s.ImageSet.ImageId : s.ImageId))
                .ForMember(d => d.Objective, opt => opt.MapFrom(s => s.ObjectiveSet.Objectives.FirstOrDefault()));

            CreateMap<Data.Models.Planetside.Objective, Domain.Models.DirectivesOutlineObjective>()
                .ForMember(d => d.TypeId, opt => opt.MapFrom(s => s.ObjectiveTypeId));

            CreateMap<Data.Models.Planetside.CharacterDirectiveTree, Domain.Models.CharacterDirectivesOutlineTree>()
                .ForMember(d => d.CompletionDate, opt => opt.MapFrom(s => s.CompletionTimeDate));

            CreateMap<Data.Models.Planetside.CharacterDirectiveTier, Domain.Models.CharacterDirectivesOutlineTier>()
                .ForMember(d => d.CompletionDate, opt => opt.MapFrom(s => s.CompletionTimeDate));

            CreateMap<Data.Models.Planetside.CharacterDirective, Domain.Models.CharacterDirectivesOutlineDirective>()
                .ForMember(d => d.CompletionDate, opt => opt.MapFrom(s => s.CompletionTimeDate));
        }

        private class CharacterVehicleStatsResolver : IValueResolver<Data.Models.Planetside.Character, Domain.Models.CharacterDetails, IEnumerable<Domain.Models.CharacterDetailsVehicleStat>>
        {
            public IEnumerable<Domain.Models.CharacterDetailsVehicleStat> Resolve(Data.Models.Planetside.Character source, Domain.Models.CharacterDetails destination, IEnumerable<Domain.Models.CharacterDetailsVehicleStat> destMember, ResolutionContext context)
            {
                var vehicleStats = CalculateVehicleStats(source.WeaponStats);

                source.WeaponStats?.Where(a => a.VehicleId != 0 && a.ItemId == 0).GroupBy(a => a.VehicleId).ToList().ForEach(item =>
                {
                    var stats = item.FirstOrDefault();

                    var stat = vehicleStats.FirstOrDefault(a => a.VehicleId == item.Key);
                    if (stat != null)
                    {
                        stat.PilotKills = stats.Kills.GetValueOrDefault();
                        stat.PilotPlayTime = stats.PlayTime.GetValueOrDefault();
                        stat.PilotVehicleKills = stats.VehicleKills.GetValueOrDefault();
                    }
                });

                return vehicleStats;
            }

            private IEnumerable<Domain.Models.CharacterDetailsVehicleStat> CalculateVehicleStats(IEnumerable<Data.Models.Planetside.CharacterWeaponStat> weaponStats)
            {
                foreach (var s in weaponStats?.Where(a => a.VehicleId != 0).GroupBy(a => a.VehicleId))
                {
                    var vehicleWeaponStats = s.Where(a => a.ItemId != 0);
                    var vehicleStats = s.FirstOrDefault(a => a.ItemId == 0);

                    yield return new Domain.Models.CharacterDetailsVehicleStat
                    {
                        // Gunner stats
                        VehicleId = s.Key,
                        DamageGiven = vehicleWeaponStats.Sum(a => a.DamageGiven.GetValueOrDefault()),
                        DamageTakenBy = vehicleWeaponStats.Sum(a => a.DamageTakenBy.GetValueOrDefault()),
                        Deaths = vehicleWeaponStats.Sum(a => a.Deaths.GetValueOrDefault()),
                        FireCount = vehicleWeaponStats.Sum(a => a.FireCount.GetValueOrDefault()),
                        HitCount = vehicleWeaponStats.Sum(a => a.HitCount.GetValueOrDefault()),
                        VehicleKills = vehicleWeaponStats.Sum(a => a.VehicleKills.GetValueOrDefault()),
                        Headshots = vehicleWeaponStats.Sum(a => a.Headshots.GetValueOrDefault()),
                        KilledBy = vehicleWeaponStats.Sum(a => a.KilledBy.GetValueOrDefault()),
                        Kills = vehicleWeaponStats.Sum(a => a.Kills.GetValueOrDefault()),
                        PlayTime = vehicleWeaponStats.Sum(a => a.PlayTime.GetValueOrDefault()),
                        Score = vehicleWeaponStats.Sum(a => a.Score.GetValueOrDefault()),

                        // Pilot stats
                        PilotDamageGiven = vehicleStats?.DamageGiven.GetValueOrDefault(),
                        PilotDamageTakenBy = vehicleStats?.DamageTakenBy.GetValueOrDefault(),
                        PilotDeaths = vehicleStats?.Deaths.GetValueOrDefault(),
                        PilotFireCount = vehicleStats?.FireCount.GetValueOrDefault(),
                        PilotHitCount = vehicleStats?.HitCount.GetValueOrDefault(),
                        PilotVehicleKills = vehicleStats?.VehicleKills.GetValueOrDefault(),
                        PilotHeadshots = vehicleStats?.Headshots.GetValueOrDefault(),
                        PilotKilledBy = vehicleStats?.KilledBy.GetValueOrDefault(),
                        PilotKills = vehicleStats?.Kills.GetValueOrDefault(),
                        PilotPlayTime = vehicleStats?.PlayTime.GetValueOrDefault(),
                        PilotScore = vehicleStats?.Score.GetValueOrDefault()
                    };
                }
            }
        }
    }
}
