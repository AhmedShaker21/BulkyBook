namespace BulkyBookWeb.Areas.Admin.Controllers
{
    using BulkyBook.DataAcess.Data;
    using BulkyBook.Models;
    using BulkyBook.Models.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Text;

    namespace BulkyBookWeb.Areas.Admin.Controllers
    {
        [Area("Admin")]
        [Authorize(Roles = "Chairman")]
        public class ReportsController : Controller
        {
            private readonly ApplicationDbContext _context;

            public ReportsController(ApplicationDbContext context)
            {
                _context = context;
            }

            public IActionResult Memos()
            {
                var vm = new MemosReportFilterVM
                {
                    Sectors = _context.Sectors.ToList(),
                    Departments = _context.Departments.ToList(),
                    Result = new List<Memo>()
                };

                return View(vm);
            }

            [HttpPost]
            public IActionResult Memos(MemosReportFilterVM filter)
            {
                var query = _context.Memos.AsQueryable();

                if (filter.FromDate.HasValue)
                    query = query.Where(m => m.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(m => m.CreatedAt <= filter.ToDate.Value);

                if (filter.DepartmentId.HasValue)
                    query = query.Where(m => m.ToDepartmentId == filter.DepartmentId.Value
                                          || m.FromDepartmentId == filter.DepartmentId.Value);

                if (filter.SectorId.HasValue)
                    query = query.Where(m => m.ToDepartment.SectorId == filter.SectorId.Value
                                          || m.FromDepartment.SectorId == filter.SectorId.Value);

                if (filter.Status.HasValue)
                    query = query.Where(m => m.Status == filter.Status.Value);

                filter.Sectors = _context.Sectors.ToList();
                filter.Departments = _context.Departments.ToList();
                filter.Result = query.ToList();

                return View(filter);
            }

            // CSV Export
            [HttpPost]
            public IActionResult ExportMemosCsv(MemosReportFilterVM filter)
            {
                var query = _context.Memos.AsQueryable();

                if (filter.FromDate.HasValue)
                    query = query.Where(m => m.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(m => m.CreatedAt <= filter.ToDate.Value);

                if (filter.DepartmentId.HasValue)
                    query = query.Where(m => m.ToDepartmentId == filter.DepartmentId.Value
                                          || m.FromDepartmentId == filter.DepartmentId.Value);

                if (filter.SectorId.HasValue)
                    query = query.Where(m => m.ToDepartment.SectorId == filter.SectorId.Value
                                          || m.FromDepartment.SectorId == filter.SectorId.Value);

                if (filter.Status.HasValue)
                    query = query.Where(m => m.Status == filter.Status.Value);

                var memos = query
                    .OrderBy(m => m.CreatedAt)
                    .ToList();

                var sb = new StringBuilder();
                sb.AppendLine("Id,Subject,FromDepartment,ToDepartment,Status,CreatedAt");

                foreach (var m in memos)
                {
                    sb.AppendLine($"{m.Id},\"{m.Subject}\",{m.FromDepartment?.Name},{m.ToDepartment?.Name},{m.Status},{m.CreatedAt:yyyy-MM-dd}");
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", "MemosReport.csv");
            }
        }
    }

}
