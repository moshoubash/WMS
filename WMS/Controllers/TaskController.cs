using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WMS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using WMS.Core.ViewModels;

namespace WMS.Controllers
{
    public class TaskController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;
        private IEnumerable<WorkerTask> ListOfTasks;
        private IEnumerable<WorkerTask> SearchedTasks;
        private WorkerTask TargetTask;

        public TaskController(IHttpClientFactory httpClientFactory, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AllTasks()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/WorkerTask");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfTasks = await JsonSerializer.DeserializeAsync<IEnumerable<WorkerTask>>(contentStream, options);
            }

            return View(ListOfTasks);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var workers = await _userManager.GetUsersInRoleAsync("WarehouseWorker");
            ViewBag.Workers = new SelectList(workers, "Id", "Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkerTask task)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(task),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PostAsync("/api/WorkerTask", jsonContent);

            var notification = new Notification { 
                Title = "New task Alert !",
                Content = $"{task.Name} :  {task.Description}",
                UserId = task.UserId
            };

            _applicationDbContext.Notifications.Add(notification);
            await _applicationDbContext.SaveChangesAsync();

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Task/AllTasks");
            }

            return Redirect("/Task/AllTasks");
        }

        [Route("/task/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/WorkerTask/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Task/AllTasks");
            }

            return Redirect("/Task/AllTasks");
        }

        [HttpGet]
        [Route("/task/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/WorkerTask/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetTask = await JsonSerializer.DeserializeAsync<WorkerTask>(contentStream, options);
            }

            var task = await _applicationDbContext.Tasks.FirstOrDefaultAsync(w => w.Id == id);

            var workers = await _userManager.GetUsersInRoleAsync("WarehouseWorker");
            ViewBag.Workers = new SelectList(workers, "Id", "Email");

            return View(task);
        }

        [HttpPost]
        [Route("/task/edit/{id}")]
        public async Task<IActionResult> Edit(int id, WorkerTask task)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(task),
            Encoding.UTF8,
               "application/json");

            var httpclient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpclient.PutAsync($"/api/WorkerTask/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Task/AllTasks");
            }

            return Redirect("/Task/AllTasks");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/WorkerTask/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                SearchedTasks = await JsonSerializer.DeserializeAsync<IEnumerable<WorkerTask>>(contentStream, options);
            }

            return PartialView("_TaskSearchResults", SearchedTasks);
        }
    }
}
