using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using WMS.Core;

namespace WMS.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Order> ListOfOrders;
        private IEnumerable<Order> SearchedOrders;
        private Order TargetOrder;
        private Address Address;
        private Customer Customer;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Order");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfOrders = await JsonSerializer.DeserializeAsync<IEnumerable<Order>>(contentStream, options);
            }

            return View(ListOfOrders);
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(order),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Order", jsonContent);

            var manager = await _userManager.GetUsersInRoleAsync("WarehouseManager");

            var notification = new Notification
            {
                Title = "New Order Alert !",
                Content = $"Order has been created on {order.OrderDate.ToString("MM/dd/yyyy")}",
                UserId = manager[0].Id
            };

            _applicationDbContext.Notifications.Add(notification);
            await _applicationDbContext.SaveChangesAsync();

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Order/AllOrders");
            }

            return Redirect("/Order/AllOrders");
        }

        [Route("/order/delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Order/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Order/AllOrders");
            }

            return Redirect("/Order/AllOrders");
        }

        [HttpGet]
        [Route("/order/edit/{id}")]
        public async Task<IActionResult> EditOrder(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Order/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetOrder = await JsonSerializer.DeserializeAsync<Order>(contentStream, options);
            }

            Address = _applicationDbContext.Addresses.FirstOrDefault(a => a.ID == TargetOrder.AddressId);
            ViewBag.Address = Address;

            Customer = _applicationDbContext.Customers.FirstOrDefault(ci => ci.ID == TargetOrder.CustomerID);
            ViewBag.Customer = Customer;

            return View(TargetOrder);
        }

        [HttpGet]
        [Route("/order/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Order/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetOrder = await JsonSerializer.DeserializeAsync<Order>(contentStream, options);
            }

            var order = await _applicationDbContext.Orders.Include(o => o.Address).Include(o => o.Customer).ThenInclude(c => c.ContactInfo).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.ID == TargetOrder.ID);

            /*int value = 0;
            
            if (TargetOrder.Status.ToLower() == "") { value = 10; }
            else if (TargetOrder.Status.ToLower() == "") { value = 25; }
            else if (TargetOrder.Status.ToLower() == "") { value = 50; }
            else if (TargetOrder.Status.ToLower() == "") { value = 75; }
            else { value = 100; }
            ViewBag.ProgressValue = value;*/

            order.OrderItems = _applicationDbContext.OrderItems.Where(oi => oi.OrderId == TargetOrder.ID).ToList();
            return View(order);
        }

        [HttpPost]
        [Route("/order/edit/{id}")]
        public async Task<IActionResult> EditOrder(int id, Order order)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(order),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Order/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Order/AllOrders");
            }

            return Redirect("/Order/AllOrders");
        }

        [HttpGet]
        public async Task<IActionResult> SearchOrder(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Order/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedOrders = await JsonSerializer.DeserializeAsync<IEnumerable<Order>>(contentStream, options);
            }

            return PartialView("_OrderSearchResults", SearchedOrders);
        }
    }
}
