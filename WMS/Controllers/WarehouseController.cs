using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Warehouse> ListOfWarehouses;
        private IEnumerable<Warehouse> SearchedWarehouses;
        private Warehouse TargetWarehouse;
        private Address Address;
        private ContactInfo Contact;
        private readonly ApplicationDbContext _applicationDbContext;

        public WarehouseController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllWarehouses()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Warehouse");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfWarehouses = await JsonSerializer.DeserializeAsync<IEnumerable<Warehouse>>(contentStream, options);
            }

            return View(ListOfWarehouses);
        }

        [HttpGet]
        public IActionResult CreateWarehouse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(Warehouse warehouse)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(warehouse),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Warehouse", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Warehouse/AllWarehouses");
            }

            return Redirect("/Warehouse/AllWarehouses");
        }

        [Route("/warehouse/delete/{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Warehouse/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Warehouse/AllWarehouses");
            }

            return Redirect("/Warehouse/AllWarehouses");
        }

        [HttpGet]
        [Route("/warehouse/edit/{id}")]
        public async Task<IActionResult> EditWarehouse(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Warehouse/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetWarehouse = await JsonSerializer.DeserializeAsync<Warehouse>(contentStream, options);
            }

            var warehouse = await _applicationDbContext.Warehouses
                                    .Include(w => w.Address)
                                    .Include(w => w.ContactInfo)
                                    .FirstOrDefaultAsync(w => w.ID == id);

            return View(warehouse);
        }

        [HttpPost]
        [Route("/warehouse/edit/{id}")]
        public async Task<IActionResult> EditWarehouse(int id, Warehouse warehouse)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(warehouse),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Warehouse/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Warehouse/AllWarehouses");
            }

            return Redirect("/Warehouse/AllWarehouses");
        }

        [HttpGet]
        public async Task<IActionResult> SearchWarehouse(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Warehouse/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedWarehouses = await JsonSerializer.DeserializeAsync<IEnumerable<Warehouse>>(contentStream, options);
            }

            return PartialView("_WarehouseSearchResults", SearchedWarehouses);
        }
    }
}
