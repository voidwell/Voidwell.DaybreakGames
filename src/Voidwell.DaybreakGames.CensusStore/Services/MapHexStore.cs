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
    public class MapHexStore : IMapHexStore
    {
        private readonly IMapRepository _mapRepository;
        private readonly MapHexCollection _mapHexCollection;
        private readonly IMapper _mapper;

        public MapHexStore(IMapRepository mapRepository, MapHexCollection mapHexCollection, IMapper mapper)
        {
            _mapRepository = mapRepository;
            _mapHexCollection = mapHexCollection;
            _mapper = mapper;
        }

        public string StoreName => "MapHexStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapHexsByZoneIdAsync(zoneId);
        }

        public async Task RefreshStore()
        {
            var mapHexs = await _mapHexCollection.GetCollectionAsync();

            if (mapHexs != null)
            {
                await _mapRepository.UpsertRangeAsync(mapHexs.Select(_mapper.Map<MapHex>));
            }
        }
    }
}
