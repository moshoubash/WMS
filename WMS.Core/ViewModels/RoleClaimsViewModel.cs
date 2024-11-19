using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Core
{
    public class RoleClaimsViewModel
    {
        public IdentityRole IdentityRole { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}
