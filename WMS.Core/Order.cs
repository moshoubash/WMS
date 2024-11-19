using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime OrderDate { get; set; }
        
        public int CustomerID { get; set; }
        public Customer Customer {get; set;}

        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        
        public string Status { get; set; }
        public float TotalAmount { get; set; }

        public Delivery? Delivery { get; set; }

        public List<OrderItem>? OrderItems { get; set; }
    }
}
