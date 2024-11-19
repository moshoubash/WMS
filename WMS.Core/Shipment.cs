using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Shipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "pending";
        public string? StartPoint { get; set; }
        public List<Delivery>? Deliveries { get; set; }
    }
}
