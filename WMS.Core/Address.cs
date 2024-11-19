using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Country { get; set; }
        public string? State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public int PostalCode { get; set; }
        public string? Info { get; set; }

        public Warehouse? Warehouse { get; set; }
        public Customer? Customer { get; set; }
        public Order? Order { get; set; }
    }
}
