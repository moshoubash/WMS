using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int? OrderID { get; set; }
        public Order? Order { get; set; }

        public int? ShipmentID { get; set; }
        public Shipment? Shipment { get; set; }

        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public string? DeliveryPerson { get; set; }
    }
}
