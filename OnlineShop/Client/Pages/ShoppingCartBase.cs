using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OnlineShop.Models.Dtos;
using OnlineShop.Client.Services.Contracts;
using System.Globalization;

namespace OnlineShop.Client.Pages
{
    public class ShoppingCartBase:ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

        public List<CartItemDto> ShoppingCartItems { get; set; }

        public string ErrorMessage { get; set; }

        protected string TotalPrice { get; set; }
        protected int TotalQuantity { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                CartChanged();
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }
        protected async Task DeleteCartItem_Click(int id)
        {
            var cartItemDto = await ShoppingCartService.DeleteItem(id);

            await RemoveCartItem(id);
            CartChanged();

        }

        protected async Task UpdateQtyCartItem_Click(int id, int qty)
        {
            try
            {
                if (qty > 0)
                {
                    var updateItemDto = new CartItemQtyUpdateDto
                    {
                        CartItemId = id,
                        Qty = qty
                    };

                    var returnedUpdateItemDto = await this.ShoppingCartService.UpdateQty(updateItemDto);

                    await UpdateItemTotalPrice(returnedUpdateItemDto);
                    
                    CartChanged();

                    //Hiding the Update Quality butten when clicked
                    await MakeUpdateQtyButtonVisible(id, false);


                }
                else
                {
                    var item = this.ShoppingCartItems.FirstOrDefault(i => i.Id == id);

                    if (item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        //Calls the Js function made in wwwroot/js
        protected async Task UpdateQty_Input(int id)
        {
           await MakeUpdateQtyButtonVisible(id, true);
        }

        private async Task MakeUpdateQtyButtonVisible(int id, bool visible)
        {
            await Js.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, visible);
        }

        private async Task UpdateItemTotalPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItem(cartItemDto.Id);

            if (item != null)
            {
                item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;
            }

            await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);

        }
        private void CalculateCartSummaryTotals()
        {
            SetTotalPrice();
            SetTotalQuantity();
        }

        private void SetTotalPrice()
        {
            TotalPrice = ShoppingCartItems.Sum(p => p.TotalPrice).ToString("C0", CultureInfo.CreateSpecificCulture("da-DK"));
        }
        private void SetTotalQuantity()
        {
            TotalQuantity = ShoppingCartItems.Sum(p => p.Qty);
        }

        private CartItemDto GetCartItem(int id)
        {
            return ShoppingCartItems.FirstOrDefault(i => i.Id == id);
        }

        /*Removes the CartItem from the clientSide cartItemList NOT Backend. 
        This way we dont need to call the server, so this is better for performance.
        The ShoppingCartService.Delete does the delete-server-part */
        private async Task RemoveCartItem(int id)
        { 
            var cartItemDto = GetCartItem(id);

            ShoppingCartItems.Remove(cartItemDto);

            await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);

        }
        private void CartChanged()
        { 
            CalculateCartSummaryTotals();
            ShoppingCartService.RaiseEventOnShoppingCartChanged(TotalQuantity);
        }

    }
}
