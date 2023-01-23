using AutoMapper;
using System;
using CensusModels = Voidwell.DaybreakGames.Live.CensusStream.Models;
using DataModels = Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Live.Mappers
{
    public class CensusToDataMappingProfile : Profile
    {
        public CensusToDataMappingProfile()
        {
            CreateMap<CensusModels.AchievementEarned, DataModels.Events.AchievementEarned>();

            CreateMap<CensusModels.BattlerankUp, DataModels.Events.BattlerankUp>();

            CreateMap<CensusModels.ContinentLock, DataModels.Events.ContinentLock>()
                .ForMember(d => d.PopulationVs, opt => opt.MapFrom(s => s.VsPopulation))
                .ForMember(d => d.PopulationNc, opt => opt.MapFrom(s => s.NcPopulation))
                .ForMember(d => d.PopulationTr, opt => opt.MapFrom(s => s.TrPopulation));

            CreateMap<CensusModels.ContinentUnlock, DataModels.Events.ContinentUnlock>();

            CreateMap<CensusModels.Death, DataModels.Events.Death>();

            CreateMap<CensusModels.FacilityControl, DataModels.Events.FacilityControl>();

            CreateMap<CensusModels.GainExperience, DataModels.Events.GainExperience>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.NewGuid()));

            CreateMap<CensusModels.MetagameEvent, DataModels.Events.MetagameEvent>()
                .ForMember(d => d.ZoneControlVs, opt => opt.MapFrom(s => s.FactionVs))
                .ForMember(d => d.ZoneControlNc, opt => opt.MapFrom(s => s.FactionNc))
                .ForMember(d => d.ZoneControlTr, opt => opt.MapFrom(s => s.FactionTr));

            CreateMap<CensusModels.MetagameEvent, DataModels.Alert>()
                .ForMember(d => d.MetagameInstanceId, opt => opt.MapFrom(s => s.InstanceId))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.Timestamp))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(s => s.Timestamp + TimeSpan.FromMinutes(45)))
                .ForMember(d => d.StartFactionVs, opt => opt.MapFrom(s => s.FactionVs))
                .ForMember(d => d.StartFactionNc, opt => opt.MapFrom(s => s.FactionNc))
                .ForMember(d => d.StartFactionTr, opt => opt.MapFrom(s => s.FactionTr))
                .ForMember(d => d.LastFactionVs, opt => opt.MapFrom(s => s.FactionVs))
                .ForMember(d => d.LastFactionNc, opt => opt.MapFrom(s => s.FactionNc))
                .ForMember(d => d.LastFactionTr, opt => opt.MapFrom(s => s.FactionTr));

            CreateMap<CensusModels.PlayerFacilityCapture, DataModels.Events.PlayerFacilityCapture>()
                .ForMember(d => d.OutfitId, opt => opt.MapFrom(s => s.OutfitId == "0" ? null : s.OutfitId));

            CreateMap<CensusModels.PlayerFacilityDefend, DataModels.Events.PlayerFacilityDefend>()
                .ForMember(d => d.OutfitId, opt => opt.MapFrom(s => s.OutfitId == "0" ? null : s.OutfitId));

            CreateMap<CensusModels.PlayerLogin, DataModels.Events.PlayerLogin>();

            CreateMap<CensusModels.PlayerLogout, DataModels.Events.PlayerLogout>();

            CreateMap<CensusModels.VehicleDestroy, DataModels.Events.VehicleDestroy>();
        }
    }
}
