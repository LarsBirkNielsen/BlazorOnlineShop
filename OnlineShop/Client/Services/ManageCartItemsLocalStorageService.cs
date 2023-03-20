using Blazored.LocalStorage;
using OnlineShop.Models.Dtos;
using OnlineShop.Client.Services.Contracts;
using OnlineShop.Client;

namespace OnlineShop.Client.Services
{
    public class ManageCartItemsLocalStorageService : IManageCartItemsLocalStorageService
    {
        const string key = "CartItemCollection";
        private readonly ILocalStorageService _localStorageService;
        private readonly IShoppingCartService _shoppingCartService;

        public ManageCartItemsLocalStorageService(ILocalStorageService localStorageService,
                                                  IShoppingCartService shoppingCartService)
        {
            _localStorageService = localStorageService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<List<CartItemDto>> GetCollection()
        {
            return await _localStorageService.GetItemAsync<List<CartItemDto>>(key)
                    ?? await AddCollection();
        }

        public async Task RemoveCollection()
        {
           await _localStorageService.RemoveItemAsync(key);
        }

        public async Task SaveCollection(List<CartItemDto> cartItemDtos)
        {
            await _localStorageService.SetItemAsync(key,cartItemDtos);
        }

        private async Task<List<CartItemDto>> AddCollection()
        {
            var shoppingCartCollection = await _shoppingCartService.GetItems(FakeUserAndCart.UserId);

            if(shoppingCartCollection != null)
            {
                await _localStorageService.SetItemAsync(key, shoppingCartCollection);
            }
            
            return shoppingCartCollection;

        }

    }
}
