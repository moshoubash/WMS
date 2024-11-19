using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
        
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public int? ContactInfoId { get; set; }
        public ContactInfo? ContactInfo { get; set; }

        public List<Inventory>? Inventories { get; set; }
    }
}
