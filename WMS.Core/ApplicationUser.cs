using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WMS.Core
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        public string? ProfilePicture { get; set; }

        public List<Notification>? Notifications { get; set; }
        public List<WorkerTask>? Tasks { get; set; }
    }
}
