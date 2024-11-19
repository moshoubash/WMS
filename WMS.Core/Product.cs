using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public string SKU { get; set; } // Stock Keeping Unit
        public decimal Price { get; set; }
        public string? Image { get; set; }

        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        public List<Inventory>? Inventories { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
