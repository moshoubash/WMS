using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // api/User
        [HttpGet]
        public async Task<IActionResult> GetUsers() {
            return Ok(await _context.Users.ToListAsync());
        }

        // api/User/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string Id)
        {
            return Ok(await _context.Users.Where(u => u.Id == Id).FirstOrDefaultAsync());
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchUser(string Query)
        {
            var user = await _context.Users.Where(u => u.Id.Contains(Query)).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
