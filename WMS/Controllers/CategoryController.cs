using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;

namespace WMS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Category> ListOfCategories;
        private IEnumerable<Category> SearchedCategories;
        private Category TargetCategory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> AllCategories()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Category");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfCategories = await JsonSerializer.DeserializeAsync<IEnumerable<Category>>(contentStream, options);
            }

            return View(ListOfCategories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(category),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Category", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Category/AllCategories");
            }

            return Redirect("/Home/Index");
        }

        [Route("/category/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Category/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Category/AllCategories");
            }

            return Redirect("/Category/AllCategories");
        }

        [HttpGet]
        [Route("/category/edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Category/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetCategory = await JsonSerializer.DeserializeAsync<Category>(contentStream, options);
            }

            return View(TargetCategory);
        }

        [HttpPost]
        [Route("/category/edit/{id}")]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(category),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Category/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Category/AllCategories");
            }

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public async Task<IActionResult> SearchCategory(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Category/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedCategories = await JsonSerializer.DeserializeAsync<IEnumerable<Category>>(contentStream, options);
            }

            return PartialView("_CategorySearchResults", SearchedCategories);
        }
    }
}
