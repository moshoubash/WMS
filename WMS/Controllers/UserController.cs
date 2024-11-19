using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text.Json;
using WMS.Api.Models;
using WMS.Core;

namespace WMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private List<ApplicationUser> ListOfUsers;
        private List<IdentityRole> ListOfRoles;
        private ApplicationUser TargetUser;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync("/api/User");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                ListOfUsers = await JsonSerializer.DeserializeAsync<List<ApplicationUser>>(contentStream, options);
            }

            var userRoles = new List<UserRolesViewModel>();

            if (ListOfUsers != null) {
                foreach (var user in ListOfUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userRoles.Add(new UserRolesViewModel
                    {
                        User = user,
                        Roles = roles
                    });
                }
            }

            return View(userRoles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            user.UserName = user.Email;

            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.UserName.ToUpper();
            user.ProfilePicture = "/img/admin-icon.png";
            await _userManager.CreateAsync(user);

            return RedirectToAction("All");
        }

        [Route("/user/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);

            if (user != null) {
                await _userManager.DeleteAsync(user);
            }
            
            return RedirectToAction("All");
        }

        [HttpGet]
        [Route("/user/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/User/{id}");
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetUser = await JsonSerializer.DeserializeAsync<ApplicationUser>(contentStream, options);
            }

            return View(TargetUser);
        }

        [HttpPost]
        [Route("/user/edit/{id}")]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            user.UserName = user.Email;

            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.UserName.ToUpper();

            var targetUser = await _userManager.FindByIdAsync(id);

            targetUser.Email = user.Email;
            targetUser.PhoneNumber = user.PhoneNumber;
            targetUser.FirstName = user.FirstName;
            targetUser.LastName = user.LastName;


            await _userManager.UpdateAsync(targetUser);

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> SearchUser(string query)
        {
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage response = await httpClient.GetAsync($"/api/User/query?Query={query}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TargetUser = await JsonSerializer.DeserializeAsync<ApplicationUser>(contentStream, options);
            }

            var userRoles = new List<UserRolesViewModel>();

            if (TargetUser != null)
            {
                var roles = await _userManager.GetRolesAsync(TargetUser);
                userRoles.Add(new UserRolesViewModel
                {
                    User = TargetUser,
                    Roles = roles
                });
            }

            return PartialView("_UserSearchResults", userRoles);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole()
        {
            // Call to get users
            var httpClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage userResponse = await httpClient.GetAsync("/api/User");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (userResponse.IsSuccessStatusCode)
            {
                using var userContentStream = await userResponse.Content.ReadAsStreamAsync();
                ListOfUsers = await JsonSerializer.DeserializeAsync<List<ApplicationUser>>(userContentStream, options);
            }

            ViewBag.Users = new SelectList(ListOfUsers, "Id", "Email");

            // Separate call to get roles
            var roleClient = _httpClientFactory.CreateClient("WMSApi");
            using HttpResponseMessage roleResponse = await roleClient.GetAsync("/api/Role");

            if (roleResponse.IsSuccessStatusCode)
            {
                using var roleContentStream = await roleResponse.Content.ReadAsStreamAsync();
                ListOfRoles = await JsonSerializer.DeserializeAsync<List<IdentityRole>>(roleContentStream, options);
            }

            ViewBag.Roles = new SelectList(ListOfRoles, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string RoleId, string UserId) {
            var role = await _roleManager.FindByIdAsync(RoleId);
            var user = await _userManager.FindByIdAsync(UserId);
            
            if (role != null && user != null)
                await _userManager.AddToRoleAsync(user, role.Name);
            
            return RedirectToAction("All");
        }

        public async Task<IActionResult> RemoveUserFromRole(string RoleId, string UserId) {
            var role = await _roleManager.FindByIdAsync(RoleId);
            var user = await _userManager.FindByIdAsync(UserId);

            if (role != null && user != null)
                await _userManager.RemoveFromRoleAsync(user, role.Name);

            return RedirectToAction("All");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Settings() {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        [Route("user/settings/{id}")]
        public async Task<IActionResult> Settings(ApplicationUser user, IFormFile ProfilePicture) {
            if (ProfilePicture != null)
            {
                var wwroot = _webHostEnvironment.WebRootPath + "/ProductsImages";
                var guid = Guid.NewGuid();
                var path = Path.Combine(wwroot, guid + ProfilePicture.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);
                }

                user.ProfilePicture = guid + ProfilePicture.FileName;
            }
            else
            {
                user.ProfilePicture = null;
            }

            user.UserName = user.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.UserName.ToUpper();

            var targetUser = await _userManager.FindByIdAsync(user.Id);

            targetUser.Email = user.Email;
            targetUser.PhoneNumber = user.PhoneNumber;
            targetUser.FirstName = user.FirstName;
            targetUser.LastName = user.LastName;

            await _userManager.UpdateAsync(targetUser);
            return RedirectToAction("Settings");
        }

        [HttpGet]
        [Authorize]
        [Route("user/Profile")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (user != null)
            {
                var userWithRoles = new UserRolesViewModel();
                userWithRoles.User = user;
                userWithRoles.Roles = roles;
                return View(userWithRoles);
            }

            else {
                return BadRequest("User is null !");
            }
        }
    }
}
