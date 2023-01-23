using AutoMapper;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Mappers
{
    public class CensusToDomainMappingProfile : Profile
    {
        public CensusToDomainMappingProfile()
        {
            CreateMap<CensusCharacterModel, CharacterSearchResult>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.CharacterId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name.First))
                .ForMember(d => d.BattleRank, opt => opt.MapFrom(s => s.BattleRank.Value))
                .ForMember(d => d.LastLogin, opt => opt.MapFrom(s => s.Times.LastLoginDate));
        }
    }
}
