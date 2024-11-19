using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Inventory> ListOfInventories;
        private IEnumerable<Inventory> SearchedInventories;
        private Inventory TargetInventory;
        private readonly ApplicationDbContext _applicationDbContext;

        public InventoryController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllInventories()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Inventory");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfInventories = await JsonSerializer.DeserializeAsync<IEnumerable<Inventory>>(contentStream, options);
            }

            return View(ListOfInventories);
        }

        [HttpGet]
        public async Task<IActionResult> CreateInventory()
        {
            var products = await _applicationDbContext.Products.ToListAsync();
            ViewBag.Products = new SelectList(products, "ProductID", "Name");

            var warehouses = await _applicationDbContext.Warehouses.ToListAsync();
            ViewBag.Warehouses = new SelectList(warehouses, "ID", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory(Inventory inventory)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");

            var jsonContent = new StringContent(JsonSerializer.Serialize(inventory), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/Inventory", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("AllInventories");
            }

            return Redirect("/Inventory/AllInventories");
        }

        [Route("/inventory/delete/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Inventory/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Inventory/AllInventories");
            }

            return Redirect("/Inventory/AllInventories");
        }

        [HttpGet]
        [Route("/inventory/edit/{id}")]
        public async Task<IActionResult> EditInventory(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Inventory/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetInventory = await JsonSerializer.DeserializeAsync<Inventory>(contentStream, options);
            }

            var products = await _applicationDbContext.Orders.ToListAsync();
            var warehouses = await _applicationDbContext.Shipments.ToListAsync();

            ViewBag.Products = new SelectList(products, "ProductID", "ProductID");
            ViewBag.Warehouses = new SelectList(warehouses, "ID", "ID");

            return View(TargetInventory);
        }

        [HttpPost]
        [Route("/inventory/edit/{id}")]
        public async Task<IActionResult> EditInventory(int id, Inventory inventory)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(inventory),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Inventory/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Inventory/AllInventories");
            }

            return Redirect("/Inventory/AllInventories");
        }

        [HttpGet]
        public async Task<IActionResult> SearchInventory(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Inventory/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedInventories = await JsonSerializer.DeserializeAsync<IEnumerable<Inventory>>(contentStream, options);
            }

            return PartialView("_InventorySearchResults", SearchedInventories);
        }
    }
}
