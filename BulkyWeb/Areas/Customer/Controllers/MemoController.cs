using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class MemoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // إنشاء مذكرة جديدة
        // ================================
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Memo model)
        {
            // 🟢 الخطوة 1: املأ القيم REQUIRED قبل الـ Validation
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null && user.DepartmentId != null)
            {
                model.FromDepartmentId = user.DepartmentId.Value;
                model.CreatedByUserId = userId;
                model.CreatedAt = DateTime.Now;
            }
            else
            {
                TempData["error"] = "المستخدم غير مرتبط بإدارة!";
                return RedirectToAction("Create", "Memo", new { area = "Admin" });
            }
            // 🟢 الخطوة 2: دلوقتي اعمل Validation


            // 🟢 الخطوة 3: احفظ البيانات
            _context.Memos.Add(model);
            _context.SaveChanges();

            TempData["success"] = "تم إرسال المذكرة!";
            return RedirectToAction("Sent");
        }
        public IActionResult Details(int id)
        {
            var memo = _context.Memos
                .Include(m => m.FromDepartment)
                .Include(m => m.ToDepartment)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            return View(memo);
        }


        // ================================
        // الوارد
        // ================================
        public IActionResult Inbox(string? search, MemoStatus? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user?.DepartmentId == null)
                return View(new List<Memo>());

            var deptId = user.DepartmentId.Value;

            var query = _context.Memos
                .Include(m => m.FromDepartment)
                .Include(m => m.ToDepartment)
                .Where(m => m.ToDepartmentId == deptId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Subject.Contains(search));

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            var memos = query.OrderByDescending(m => m.CreatedAt).ToList();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;

            return View(memos);
        }


        // ================================
        // الصادر
        // ================================
        public IActionResult Sent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var memos = _context.Memos
                .Include(m => m.FromDepartment)
                .Include(m => m.ToDepartment)
                .Where(m => m.CreatedByUserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return View(memos);
        }


        // ================================
        // التحويل للمراجعة
        // ================================
        [Authorize(Roles = "DepartmentManager, GeneralManager, SectorManager, Chairman")]
        public IActionResult Review(int id)
        {
            var memo = _context.Memos
                 .Include(m => m.FromDepartment)
                 .Include(m => m.ToDepartment)
                 .Include(m => m.CreatedByUserId)
                 .FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            memo.Status = MemoStatus.UnderReview;
            memo.ReviewedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _context.SaveChanges();

            TempData["success"] = "تم تحويل المذكرة إلى المراجعة ✔";
            return RedirectToAction(nameof(Inbox));
        }

        [Authorize(Roles = "DepartmentManager, GeneralManager, SectorManager, Chairman")]
        public IActionResult Approve(int id)
        {
            var memo = _context.Memos.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            memo.Status = MemoStatus.Approved;
            memo.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            _context.SaveChanges();

            CreateNotification(
                memo.CreatedByUserId,
                "تمت الموافقة على المذكرة",
                $"تمت الموافقة على: {memo.Subject}",
                "/Customer/Memo/Sent"
            );

            TempData["success"] = "تمت الموافقة ✔";
            return RedirectToAction(nameof(Inbox));
        }


        // ================================
        // الرفض
        // ================================
        [Authorize(Roles = "DepartmentManager, GeneralManager, SectorManager, Chairman")]
        public IActionResult Reject(int id)
        {
            var memo = _context.Memos.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var oldStatus = memo.Status;

            memo.Status = MemoStatus.Rejected;
            memo.RejectedByUserId = currentUserId;

            _context.SaveChanges();

            CreateNotification(
                memo.CreatedByUserId,
                "تم رفض المذكرة",
                $"تم رفض المذكرة: {memo.Subject}",
                "/Customer/Memo/Sent"
            );

            LogActivity(currentUserId, memo, "Rejected", oldStatus, memo.Status, "تم الرفض");

            TempData["error"] = "تم رفض المذكرة ❌";
            return RedirectToAction(nameof(Inbox));
        }



        // ================================
        // الإغلاق
        // ================================
        [Authorize(Roles = "Chairman")]
        public IActionResult Close(int id)
        {
            var memo = _context.Memos.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var oldStatus = memo.Status;

            memo.Status = MemoStatus.Closed;
            memo.ClosedByUserId = currentUserId;

            _context.SaveChanges();

            CreateNotification(
                memo.CreatedByUserId,
                "تم إغلاق المذكرة",
                $"تم إغلاق المذكرة: {memo.Subject}",
                "/Customer/Memo/Sent"
            );

            LogActivity(currentUserId, memo, "Closed", oldStatus, memo.Status, "إغلاق نهائي");

            TempData["success"] = "تم إغلاق المذكرة ✔";
            return RedirectToAction(nameof(Inbox));
        }


        // ================================
        // Helpers
        // ================================
        private void CreateNotification(string userId, string title, string message, string? link = null)
        {
            var notif = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Link = link
            };

            _context.Notifications.Add(notif);
            _context.SaveChanges();
        }

        private void LogActivity(
            string userId,
            Memo memo,
            string action,
            MemoStatus? oldStatus,
            MemoStatus? newStatus,
            string? note = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                MemoId = memo.Id,
                Action = action,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Note = note
            };

            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
