using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _roles;
        public StatisticsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> roles)
        {
            _dbContext = dbContext;
            _roles = roles;
        }
        
        [HttpGet("Statistics")]
        public async Task<IActionResult> GetAllStats()
        {
            var workers = await _roles.GetUsersInRoleAsync("WarehouseWorker");
            var data = new Dictionary<string, int>
            {
                { "orders", _dbContext.Orders.Count() },
                { "customers", _dbContext.Customers.Count() },
                { "products", _dbContext.Products.Count() },
                { "workers", workers.Count() }
            };

            return Ok(data);
        }
    }
}
