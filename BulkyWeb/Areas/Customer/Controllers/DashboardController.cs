using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Chairman"))
                return RedirectToAction("ChairmanDashboard");

            if (User.IsInRole("SectorManager"))
                return RedirectToAction("SectorDashboard");

            if (User.IsInRole("DepartmentManager") ||
                User.IsInRole("GeneralManager"))
                return RedirectToAction("DepartmentDashboard");

            return RedirectToAction("EmployeeDashboard");
        }

        //--------------------------------------
        // 1️⃣ Chairman Dashboard
        //--------------------------------------
        [Authorize(Roles = "Chairman")]
        public IActionResult ChairmanDashboard()
        {
            var vm = new DashboardVM
            {
                SectorsCount = _context.Sectors.Count(),
                DepartmentsCount = _context.Departments.Count(),
                UsersCount = _context.Users.Count(),
                MemosCount = _context.Memos.Count(),

                NewMemos = _context.Memos.Count(m => m.Status == MemoStatus.New),
                UnderReview = _context.Memos.Count(m => m.Status == MemoStatus.UnderReview),
                Approved = _context.Memos.Count(m => m.Status == MemoStatus.Approved),
                Rejected = _context.Memos.Count(m => m.Status == MemoStatus.Rejected),
                Closed = _context.Memos.Count(m => m.Status == MemoStatus.Closed),

                LatestMemos = _context.Memos
                    .Include(m => m.FromDepartment)
                    .Include(m => m.ToDepartment)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList(),
            };

            // -----------------------------
            // FIX Monthly Stats
            // -----------------------------
            var monthlyRaw = _context.Memos
                .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    MonthNumber = g.Key.Month,
                    Count = g.Count()
                })
                .ToList(); // ← تحميل من SQL أولاً

            vm.MonthlyStats = monthlyRaw
                .OrderBy(m => new DateTime(m.Year, m.MonthNumber, 1))
                .Select(m => new MonthlyMemoStat
                {
                    Month = CultureInfo.InvariantCulture.DateTimeFormat
                        .GetAbbreviatedMonthName(m.MonthNumber),
                    Count = m.Count
                })
                .ToList();

            return View(vm);
        }

        //--------------------------------------
        // 2️⃣ Sector Manager
        //--------------------------------------
        [Authorize(Roles = "SectorManager")]
        public IActionResult SectorDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = _context.Users.First(u => u.Id == userId);

            var sectorId = user.SectorId;

            var vm = new SectorDashboardVM
            {
                SectorName = _context.Sectors.First(s => s.Id == sectorId).Name,

                Departments = _context.Departments
                    .Where(d => d.SectorId == sectorId)
                    .ToList(),

                EmployeesCount = _context.Users.Count(u => u.SectorId == sectorId),

                SentMemos = _context.Memos
                    .Include(m => m.FromDepartment)
                    .Where(m => m.FromDepartment.SectorId == sectorId)
                    .Count(),

                ReceivedMemos = _context.Memos
                    .Include(m => m.ToDepartment)
                    .Where(m => m.ToDepartment.SectorId == sectorId)
                    .Count(),

                LatestMemos = _context.Memos
                    .Include(m => m.FromDepartment)
                    .Include(m => m.ToDepartment)
                    .Where(m => m.FromDepartment.SectorId == sectorId ||
                                m.ToDepartment.SectorId == sectorId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }

        //--------------------------------------
        // 3️⃣ Department Manager
        //--------------------------------------
        [Authorize(Roles = "DepartmentManager,GeneralManager")]
        public IActionResult DepartmentDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = _context.Users.First(u => u.Id == userId);

            var deptId = user.DepartmentId;

            var vm = new DepartmentDashboardVM
            {
                DepartmentName = _context.Departments.First(d => d.Id == deptId).Name,

                EmployeesCount = _context.Users.Count(u => u.DepartmentId == deptId),

                Inbox = _context.Memos.Count(m => m.ToDepartmentId == deptId),
                Outbox = _context.Memos.Count(m => m.FromDepartmentId == deptId),

                ReviewCount = _context.Memos.Count(m => m.ReviewedByUserId == userId),
                ApprovedCount = _context.Memos.Count(m => m.ApprovedByUserId == userId),
                RejectedCount = _context.Memos.Count(m => m.RejectedByUserId == userId),

                LatestMemos = _context.Memos
                    .Include(m => m.FromDepartment)
                    .Include(m => m.ToDepartment)
                    .Where(m => m.ToDepartmentId == deptId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }

        //--------------------------------------
        // 4️⃣ Employee Dashboard
        //--------------------------------------
        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var vm = new EmployeeDashboardVM
            {
                Sent = _context.Memos.Count(m => m.CreatedByUserId == userId),
                Approved = _context.Memos.Count(m => m.CreatedByUserId == userId && m.Status == MemoStatus.Approved),
                Rejected = _context.Memos.Count(m => m.CreatedByUserId == userId && m.Status == MemoStatus.Rejected),

                LatestMemos = _context.Memos
                    .Where(m => m.CreatedByUserId == userId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}
