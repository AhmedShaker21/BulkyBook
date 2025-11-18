using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // عرض جميع الإدارات مع البحث بالقطاع
        // =====================================================
        public IActionResult Index(string searchSector)
        {
            var departments = _context.Departments
                .Include(d => d.Sector)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchSector))
            {
                departments = departments.Where(d =>
                    d.Sector.Name.Contains(searchSector));
            }

            return View(departments.ToList());
        }

        // =====================================================
        // صفحة إنشاء إدارة جديدة (GET)
        // =====================================================
        public IActionResult Create()
        {
            ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name");
            return View();
        }

        // =====================================================
        // حفظ الإدارة الجديدة (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name");
                return View(model);
            }

            _context.Departments.Add(model);
            _context.SaveChanges();

            TempData["success"] = "تم إضافة الإدارة بنجاح!";
            return RedirectToAction("Index");
        }

        // =====================================================
        // صفحة تعديل الإدارة (GET)
        // =====================================================
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
                return NotFound();

            ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name", department.SectorId);

            return View(department);
        }

        // =====================================================
        // تعديل الإدارة (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department model)
        {
           

            _context.Departments.Update(model);
            _context.SaveChanges();

            TempData["success"] = "تم تحديث الإدارة بنجاح!";
            return RedirectToAction("Index");
        }

        // =====================================================
        // صفحة الحذف (GET)
        // =====================================================
        public IActionResult Delete(int id)
        {
            var department = _context.Departments
                                     .Include(d => d.Sector)
                                     .FirstOrDefault(d => d.Id == id);

            if (department == null)
                return NotFound();

            return View(department);
        }


        // =====================================================
        // حذف القسم فعليًا (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                TempData["error"] = "الإدارة غير موجودة!";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();

                TempData["success"] = "تم حذف الإدارة بنجاح!";
            }
            catch (Exception)
            {
                TempData["error"] = "لا يمكن حذف الإدارة لأنها مرتبطة ببيانات أخرى!";
            }

            return RedirectToAction("Index");
        }

    }
}
