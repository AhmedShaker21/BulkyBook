namespace BulkyBookWeb.Areas.Customer.Controllers
{
    using BulkyBook.DataAcess.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace BulkyBookWeb.Areas.Customer.Controllers
    {
        [Area("Customer")]
        [Authorize]
        public class NotificationsController : Controller
        {
            private readonly ApplicationDbContext _context;

            public NotificationsController(ApplicationDbContext context)
            {
                _context = context;
            }

            public IActionResult Index()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var notifs = _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                return View(notifs);
            }

            public IActionResult MarkAsRead(int id)
            {
                var notif = _context.Notifications.FirstOrDefault(n => n.Id == id);
                if (notif == null) return NotFound();

                notif.IsRead = true;
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(notif.Link))
                    return Redirect(notif.Link);

                return RedirectToAction("Index");
            }

            public IActionResult MarkAllAsRead()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var notifs = _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToList();

                foreach (var n in notifs) n.IsRead = true;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }

}
