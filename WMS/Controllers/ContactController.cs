using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;

namespace WMS.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<ContactInfo> ListOfContacts;
        private IEnumerable<ContactInfo> SearchedContacts;
        private ContactInfo TargetContactInfo;

        public ContactController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> AllContacts()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/ContactInfo");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfContacts = await JsonSerializer.DeserializeAsync<IEnumerable<ContactInfo>>(contentStream, options);
            }

            return View(ListOfContacts);
        }

        [HttpGet]
        public IActionResult CreateContact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactInfo contact)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(contact),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/ContactInfo", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Contact/AllContacts");
            }

            return Redirect("/Home/Index");
        }

        [Route("/contact/delete/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/ContactInfo/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Contact/AllContacts");
            }

            return Redirect("/Contact/AllContacts");
        }

        [HttpGet]
        [Route("/contact/edit/{id}")]
        public async Task<IActionResult> EditContact(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/ContactInfo/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetContactInfo = await JsonSerializer.DeserializeAsync<ContactInfo>(contentStream, options);
            }

            return View(TargetContactInfo);
        }

        [HttpPost]
        [Route("/contact/edit/{id}")]
        public async Task<IActionResult> EditContact(int id, ContactInfo contact)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(contact),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/ContactInfo/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Contact/AllContacts");
            }

            return Redirect("/Contact/AllContacts");
        }

        [HttpGet]
        public async Task<IActionResult> SearchContact(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/ContactInfo/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedContacts = await JsonSerializer.DeserializeAsync<IEnumerable<ContactInfo>>(contentStream, options);
            }

            return PartialView("_ContactSearchResults", SearchedContacts);
        }
    }
}
