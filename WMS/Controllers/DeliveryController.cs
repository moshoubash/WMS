using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Delivery> ListOfDeliveries;
        private IEnumerable<Delivery> SearchedDeliveries;
        private Delivery TargetDelivery;
        private readonly ApplicationDbContext _applicationDbContext;

        public DeliveryController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllDeliveries()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Delivery");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfDeliveries = await JsonSerializer.DeserializeAsync<IEnumerable<Delivery>>(contentStream, options);
            }

            return View(ListOfDeliveries);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDelivery()
        {
            var orders = await _applicationDbContext.Orders.ToListAsync();
            var shipments = await _applicationDbContext.Shipments.ToListAsync();
            
            ViewBag.Orders = new SelectList(orders, "ID", "ID");
            ViewBag.Shipments = new SelectList(shipments, "ID", "Date");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDelivery(Delivery delivery)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(delivery),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Delivery", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Delivery/AllDeliveries");
            }

            return Redirect("/Delivery/AllDeliveries");
        }

        [Route("/delivery/delete/{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Delivery/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Delivery/AllDeliveries");
            }

            return Redirect("/Delivery/AllDeliveries");
        }

        [HttpGet]
        [Route("/delivery/edit/{id}")]
        public async Task<IActionResult> EditDelivery(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Delivery/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetDelivery = await JsonSerializer.DeserializeAsync<Delivery>(contentStream, options);
            }

            var orders = await _applicationDbContext.Orders.ToListAsync();
            var shipments = await _applicationDbContext.Shipments.ToListAsync();

            ViewBag.Orders = new SelectList(orders, "ID", "ID");
            ViewBag.Shipments = new SelectList(shipments, "ID", "Date");
            return View(TargetDelivery);
        }

        [HttpPost]
        [Route("/delivery/edit/{id}")]
        public async Task<IActionResult> EditDelivery(int id, Delivery delivery)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(delivery),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Delivery/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Delivery/AllDeliveries");
            }

            return Redirect("/Delivery/AllDeliveries");
        }

        [HttpGet]
        public async Task<IActionResult> SearchDelivery(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Delivery/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedDeliveries = await JsonSerializer.DeserializeAsync<IEnumerable<Delivery>>(contentStream, options);
            }

            return PartialView("_DeliverySearchResults", SearchedDeliveries);
        }
    }
}
