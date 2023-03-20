using Microsoft.AspNetCore.Components;
using OnlineShop.Models.Dtos;

namespace OnlineShop.Client.Pages
{
    public class DisplayProductsBase:ComponentBase
    {
        [Parameter]
        public IEnumerable<ProductDto> Products { get; set; }
    
    }
}
