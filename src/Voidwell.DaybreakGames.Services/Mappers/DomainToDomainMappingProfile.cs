using AutoMapper;
using System.Linq;

namespace Voidwell.DaybreakGames.Services.Mappers
{
    public class DomainToDomainMappingProfile : Profile
    {
        public DomainToDomainMappingProfile()
        {
            CreateMap<Domain.Models.CharacterDetails, Domain.Models.SimpleCharacterDetails>()
                .ForMember(d => d.LastSaved, opt => opt.MapFrom(s => s.Times.LastSaveDate))
                .ForMember(d => d.FactionName, opt => opt.MapFrom(s => s.Faction))
                .ForMember(d => d.OutfitAlias, opt => opt.MapFrom(s => s.Outfit.Alias))
                .ForMember(d => d.OutfitName, opt => opt.MapFrom(s => s.Outfit.Name))
                .ForMember(d => d.Kills, opt => opt.MapFrom(s => s.LifetimeStats.Kills))
                .ForMember(d => d.Deaths, opt => opt.MapFrom(s => s.LifetimeStats.Deaths))
                .ForMember(d => d.Score, opt => opt.MapFrom(s => s.LifetimeStats.Score))
                .ForMember(d => d.PlayTime, opt => opt.MapFrom(s => s.LifetimeStats.PlayTime))
                .ForMember(d => d.TotalPlayTimeMinutes, opt => opt.MapFrom(s => s.Times.MinutesPlayed))
                .ForMember(d => d.IVIScore, opt => opt.MapFrom(s => s.InfantryStats.IVIScore))
                .ForMember(d => d.IVIKillDeathRatio, opt => opt.MapFrom(s => s.InfantryStats.KillDeathRatio))
                .ForMember(d => d.Prestige, opt => opt.MapFrom(s => s.PrestigeLevel))
                .ForMember(d => d.PlayTimeInMax, opt => opt.MapFrom(s => s.ProfileStats.FirstOrDefault(a => a.ProfileId == 7).PlayTime));
        }
    }
}
