using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;

namespace WMS.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Customer> ListOfCustomers;
        private IEnumerable<Customer> SearchedCustomers;
        private Customer TargetCustomer;
        private Address Address;
        private ContactInfo Contact;
        private readonly ApplicationDbContext _applicationDbContext;

        public CustomerController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllCustomers()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Customer");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfCustomers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentStream, options);
            }

            return View(ListOfCustomers);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(customer),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Customer", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Customer/AllCustomers");
            }

            return Redirect("/Customer/AllCustomers");
        }

        [Route("/customer/delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Customer/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Customer/AllCustomers");
            }

            return Redirect("/Customer/AllCustomers");
        }

        [HttpGet]
        [Route("/customer/edit/{id}")]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Customer/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetCustomer = await JsonSerializer.DeserializeAsync<Customer>(contentStream, options);
            }

            Address = _applicationDbContext.Addresses.FirstOrDefault(a => a.ID == TargetCustomer.AddressId);
            ViewBag.Address = Address;

            Contact = _applicationDbContext.ContactInfos.FirstOrDefault(ci => ci.ID == TargetCustomer.ContactInfoId);
            ViewBag.Contact = Contact;

            return View(TargetCustomer);
        }

        [HttpPost]
        [Route("/customer/edit/{id}")]
        public async Task<IActionResult> EditCustomer(int id, Customer customer)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(customer),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Customer/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Customer/AllCustomers");
            }

            return Redirect("/Customer/AllCustomers");
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomer(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Customer/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedCustomers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentStream, options);
            }

            return PartialView("_CustomerSearchResults", SearchedCustomers);
        }
    }
}
