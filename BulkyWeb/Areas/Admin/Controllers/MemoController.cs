using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.DataAcess.Data;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MemoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------------------------
        // GET: Admin/Memo/Create
        // -------------------------------------
        [HttpGet]
        public IActionResult Create()
        {
            // تحميل الإدارات ليتم اختيار "الجهة المستقبلة"
            ViewBag.Departments = new SelectList(
                _context.Departments.ToList(),
                "Id",
                "Name"
            );

            return View();
        }
    }
}
