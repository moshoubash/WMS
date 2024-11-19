using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Core
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string? Content { get; set; }

        public bool IsRead { get; set; } = false;
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
