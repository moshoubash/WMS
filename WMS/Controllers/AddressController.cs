using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using WMS.Core;

namespace WMS.Controllers
{
    public class AddressController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Address> ListOfAddresses;
        private IEnumerable<Address> SearchedAddresses;
        private Address TargetAddress;

        public AddressController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> AllAddresses()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Address");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfAddresses = await JsonSerializer.DeserializeAsync<IEnumerable<Address>>(contentStream, options);
            }

            return View(ListOfAddresses);
        }

        [HttpGet]
        public IActionResult CreateAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(Address address)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(address),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Address", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Address/AllAddresses");
            }

            return Redirect("/Home/Index");
        }

        [Route("/address/delete/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Address/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Address/AllAddresses");
            }

            return Redirect("/Address/AllAddresses");
        }

        [HttpGet]
        [Route("/address/edit/{id}")]
        public async Task<IActionResult> EditAddress(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Address/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetAddress = await JsonSerializer.DeserializeAsync<Address>(contentStream, options);
            }

            return View(TargetAddress);
        }

        [HttpPost]
        [Route("/address/edit/{id}")]
        public async Task<IActionResult> EditAddress(int id, Address address)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(address),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Address/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Address/AllAddresses");
            }

            return Redirect("/Address/AllAddresses");
        }

        [HttpGet]
        public async Task<IActionResult> SearchAddress(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Address/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedAddresses = await JsonSerializer.DeserializeAsync<IEnumerable<Address>>(contentStream, options);
            }

            return PartialView("_AddressSearchResults", SearchedAddresses);
        }
    }
}
