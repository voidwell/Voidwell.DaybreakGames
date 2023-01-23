using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class FacilityLinkStore : IFacilityLinkStore
    {
        private readonly IMapRepository _mapRepository;
        private readonly FacilityLinkCollection _facilityLinkCollection;
        private readonly IMapper _mapper;

        public FacilityLinkStore(IMapRepository mapRepository, FacilityLinkCollection facilityLinkCollection, IMapper mapper)
        {
            _mapRepository = mapRepository;
            _facilityLinkCollection = facilityLinkCollection;
            _mapper = mapper;
        }

        public string StoreName => "FacilityLinkStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
        }

        public async Task RefreshStore()
        {
            var facilityLinks = await _facilityLinkCollection.GetCollectionAsync();

            if (facilityLinks != null)
            {
                await _mapRepository.UpsertRangeAsync(facilityLinks.Select(_mapper.Map<FacilityLink>));
            }
        }
    }
}
