using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;

namespace WMS.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Shipment> ListOfShipments;
        private IEnumerable<Shipment> SearchedShipments;
        private Shipment TargetShipment;
        private readonly ApplicationDbContext _applicationDbContext;

        public ShipmentController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllShipments()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Shipment");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfShipments = await JsonSerializer.DeserializeAsync<IEnumerable<Shipment>>(contentStream, options);
            }

            return View(ListOfShipments);
        }

        [HttpGet]
        public IActionResult CreateShipment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment(Shipment shipment)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(shipment),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Shipment", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Shipment/AllShipments");
            }

            return Redirect("/Shipment/AllShipments");
        }

        [Route("/shipment/delete/{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Shipment/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Shipment/AllShipments");
            }

            return Redirect("/Shipment/AllShipments");
        }

        [HttpGet]
        [Route("/shipment/edit/{id}")]
        public async Task<IActionResult> EditShipment(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Shipment/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetShipment = await JsonSerializer.DeserializeAsync<Shipment>(contentStream, options);
            }
            
            return View(TargetShipment);
        }

        [HttpPost]
        [Route("/shipment/edit/{id}")]
        public async Task<IActionResult> EditShipment(int id, Shipment shipment)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(shipment),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Shipment/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Shipment/AllShipments");
            }

            return Redirect("/Shipment/AllShipments");
        }

        [HttpGet]
        public async Task<IActionResult> SearchShipment(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Shipment/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedShipments = await JsonSerializer.DeserializeAsync<IEnumerable<Shipment>>(contentStream, options);
            }

            return PartialView("_ShipmentSearchResults", SearchedShipments);
        }
    }
}
