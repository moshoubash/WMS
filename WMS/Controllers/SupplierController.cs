using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WMS.Core;

namespace WMS.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Supplier> ListOfSuppliers;
        private IEnumerable<Supplier> SearchedSuppliers;
        private Supplier TargetSupplier;
        private readonly ApplicationDbContext _applicationDbContext;

        public SupplierController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllSuppliers()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Supplier");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfSuppliers = await JsonSerializer.DeserializeAsync<IEnumerable<Supplier>>(contentStream, options);
            }
            
            return View(ListOfSuppliers);
        }

        [HttpGet]
        public IActionResult CreateSupplier()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(Supplier supplier)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(supplier),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Supplier", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Supplier/AllSuppliers");
            }

            return Redirect("/Supplier/AllSuppliers");
        }

        [Route("/supplier/delete/{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Supplier/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Supplier/AllSuppliers");
            }

            return Redirect("/Supplier/AllSuppliers");
        }

        [HttpGet]
        [Route("/supplier/edit/{id}")]
        public async Task<IActionResult> EditSupplier(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Supplier/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetSupplier = await JsonSerializer.DeserializeAsync<Supplier>(contentStream, options);
            }
            ViewBag.Contact = _applicationDbContext.ContactInfos.Where(ci => ci.ID == TargetSupplier.ContactInfoId).FirstOrDefault();

            return View(TargetSupplier);
        }

        [HttpPost]
        [Route("/supplier/edit/{id}")]
        public async Task<IActionResult> EditSupplier(int id, Supplier supplier)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(supplier),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Supplier/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Supplier/AllSuppliers");
            }

            return Redirect("/Supplier/AllSuppliers");
        }

        [HttpGet]
        public async Task<IActionResult> SearchSupplier(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Supplier/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedSuppliers = await JsonSerializer.DeserializeAsync<IEnumerable<Supplier>>(contentStream, options);
            }

            return PartialView("_SupplierSearchResults", SearchedSuppliers);
        }
    }
}
