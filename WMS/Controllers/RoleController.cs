using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WMS.Api.Models;
using WMS.Core;

namespace WMS.Controllers
{
    public class RoleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private List<IdentityRole> ListOfRoles;
        private IdentityRole TargetRole;
        private ApplicationUser TargetUser;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public RoleController(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Role");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfRoles = await JsonSerializer.DeserializeAsync<List<IdentityRole>>(contentStream, options);
            }

            var roleClaims = new List<RoleClaimsViewModel>();

            if (ListOfRoles != null) {
                foreach (var role in ListOfRoles) {
                    var claims = await _roleManager.GetClaimsAsync(role);

                    roleClaims.Add(new RoleClaimsViewModel
                    {
                        IdentityRole = role,
                        Claims = claims
                    });
                }
            }

            return View(roleClaims);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            await _roleManager.CreateAsync(role);

            return RedirectToAction("All");
        }

        [Route("/role/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole? role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("All");
        }

        [HttpGet]
        [Route("/role/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Role/{id}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetRole = await JsonSerializer.DeserializeAsync<IdentityRole>(contentStream, options);
            }

            return View(TargetRole);
        }

        [HttpPost]
        [Route("/role/edit/{id}")]
        public async Task<IActionResult> Edit(string id, IdentityRole role)
        {
            var targetRole = await _roleManager.FindByIdAsync(id);

            if (targetRole == null)
            {
                return NotFound();
            }

            targetRole.Name = role.Name;
            targetRole.NormalizedName = role.Name.ToUpper();

            var result = await _roleManager.UpdateAsync(targetRole);

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            return View(role);
        }


        [HttpGet]
        public async Task<IActionResult> SearchUser(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/Role/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetRole = await JsonSerializer.DeserializeAsync<IdentityRole>(contentStream, options);
            }

            return PartialView("_RoleSearchResults", TargetRole);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleClaims(string RoleId, string Type, string Value)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var claim = new Claim(Type, Value);

            var result = await _roleManager.AddClaimAsync(role, claim);
            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            ModelState.AddModelError("", "Failed to add claim.");
            return View();
        }

        public async Task<IActionResult> DeleteClaim(string Type, string RoleId) {
            var role = await _roleManager.FindByIdAsync(RoleId);
            IList<Claim> claims = await _roleManager.GetClaimsAsync(role);

            await _roleManager.RemoveClaimAsync(role, claims.Where(c => c.Type.Contains(Type)).FirstOrDefault());

            return RedirectToAction("All");
        }
    }
}
