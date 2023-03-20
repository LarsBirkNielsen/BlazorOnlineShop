using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Server.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public ProductCategory ProductCategory { get; set; }

    }
}
