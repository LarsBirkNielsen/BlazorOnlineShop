using Blazored.LocalStorage;
using OnlineShop.Models.Dtos;
using OnlineShop.Client.Services.Contracts;

namespace OnlineShop.Client.Services
{
    public class ManageProductsLocalStorageService : IManageProductsLocalStorageService
    {


        private const string key = "ProductCollection";
        private readonly ILocalStorageService _localStorageService;
        private readonly IProductService _productService;

        public ManageProductsLocalStorageService(ILocalStorageService localStorageService,
                                                 IProductService productService)
        {
            _localStorageService = localStorageService;
            _productService = productService;
        }

        public async Task<IEnumerable<ProductDto>> GetCollection()
        {
            return await _localStorageService.GetItemAsync<IEnumerable<ProductDto>>(key)
                    ?? await AddCollection();
        }

        public async Task RemoveCollection()
        {
           await _localStorageService.RemoveItemAsync(key);
        }

        private async Task<IEnumerable<ProductDto>> AddCollection()
        {
            var productCollection = await _productService.GetItems();

            if (productCollection != null)
            {
                await _localStorageService.SetItemAsync(key, productCollection);
            }

            return productCollection;

        }

    }
}
