using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using WMS.Core;

namespace WMS.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _users;
        public AdminController(IHttpClientFactory httpClientFactory, ApplicationDbContext dbContext, UserManager<ApplicationUser> users)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
            _users = users;
        }
        public async Task<IActionResult> Dashboard()
        {
            var workers = await _users.GetUsersInRoleAsync("WarehouseWorker");

            ViewBag.Orders = _dbContext.Orders.Count();
            ViewBag.Customers = _dbContext.Customers.Count();
            ViewBag.Workers = workers.Count();
            ViewBag.Products = _dbContext.Products.Count();

            ViewBag.PendingOrders = _dbContext.Orders.Include(o => o.Customer)
                                                        .ThenInclude(c => c.ContactInfo)
                                                        .Where(o => o.Status.ToLower() == "pending")
                                                        .ToList();

            return View();
        }
    }
}
