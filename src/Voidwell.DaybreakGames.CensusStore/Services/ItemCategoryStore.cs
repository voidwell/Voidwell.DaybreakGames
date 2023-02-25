using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ItemCategoryStore : IItemCategoryStore
    {
        private readonly IItemRepository _itemRepository;
        private readonly ItemCategoryCollection _itemCategoryCollection;
        private readonly IMapper _mapper;

        public string StoreName => "ItemCategoryStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public ItemCategoryStore(IItemRepository itemRepository, ItemCategoryCollection itemCategoryCollection, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _itemCategoryCollection = itemCategoryCollection;
            _mapper = mapper;
        }

        public async Task RefreshStore()
        {
            var itemCategories = await _itemCategoryCollection.GetCollectionAsync();
            if (itemCategories != null)
            {
                await _itemRepository.UpsertRangeAsync(itemCategories.Select(_mapper.Map<ItemCategory>));
            }
        }
    }
}
