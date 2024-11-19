using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IEnumerable<Product> ListOfProducts;
        private IEnumerable<Product> SearchedProducts;
        private IEnumerable<Category> Categories;
        private Product TargetProduct;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AllProducts()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Product");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfProducts = await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(contentStream, options);
            }

            return View(ListOfProducts);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
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
                Categories = await JsonSerializer.DeserializeAsync<IEnumerable<Category>>(contentStream, options);
            }

            ViewBag.Categories = new SelectList(Categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile Image)
        {
            if (Image != null)
            {
                var wwroot = _webHostEnvironment.WebRootPath + "/ProductsImages";
                var guid = Guid.NewGuid();
                var path = Path.Combine(wwroot, guid + Image.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                product.Image = guid + Image.FileName;
            }
            else
            {
                product.Image = null;
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(product),
            Encoding.UTF8,
            "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/Product", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Product/AllProducts");
            }

            return Redirect("/Product/AllProducts");
        }

        [Route("/product/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Product/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Product/AllProducts");
            }

            return Redirect("/Product/AllProducts");
        }

        [HttpGet]
        [Route("/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Product/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetProduct = await JsonSerializer.DeserializeAsync<Product>(contentStream, options);
            }

            var Client = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage res = await Client.GetAsync("/api/Category");

            var opt = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await res.Content.ReadAsStreamAsync();
                Categories = await JsonSerializer.DeserializeAsync<IEnumerable<Category>>(contentStream, opt);
            }

            ViewBag.Categories = new SelectList(Categories, "Id", "Name");

            return View(TargetProduct);
        }

        [HttpPost]
        [Route("/product/edit/{id}")]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile Image)
        {
            if (Image != null)
            {
                var wwroot = _webHostEnvironment.WebRootPath + "/ProductsImages";
                var guid = Guid.NewGuid();
                var path = Path.Combine(wwroot, guid + Image.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                product.Image = guid + Image.FileName;
            }
            else
            {
                product.Image = null;
            }

            product.ProductID = id;

            var jsonContent = new StringContent(JsonSerializer.Serialize(product),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/Product/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Product/AllProducts");
            }

            return Redirect("/Product/AllProducts");
        }

        [HttpGet]
        [Route("/product/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Product/{id}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetProduct = await JsonSerializer.DeserializeAsync<Product>(contentStream, options);
            }

            else
            {
                return BadRequest("Can't fetch data !");
            }
            ViewBag.Category = _dbContext.Categories.Find(TargetProduct.CategoryID).Name;
            return View(TargetProduct);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Product/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedProducts = await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(contentStream, options);
            }

            return PartialView("_ProductSearchResults", SearchedProducts);
        }
    }
}
