using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class ContactInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public Customer? Customer { get; set; }
        public Supplier? Supplier { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
