using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await _context.Notifications.OrderByDescending(n => n.Time).ToListAsync());
        }

        public async Task<IActionResult> ToggleRead(int id) {
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (notification != null && notification.IsRead == false)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            else { 
                return RedirectToAction("List");
            }

            return RedirectToAction("List");
        }
    }
}
