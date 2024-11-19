using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int Quantity { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? WarehouseID { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
