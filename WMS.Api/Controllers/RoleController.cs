using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // api/Role
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _context.Roles.ToListAsync());
        }

        // api/Role/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string Id)
        {
            return Ok(await _context.Roles.Where(u => u.Id == Id).FirstOrDefaultAsync());
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchRole(string Query)
        {
            var role = await _context.Roles.Where(r => r.Id.Contains(Query) || r.Name.Contains(Query)).FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }
    }
}
