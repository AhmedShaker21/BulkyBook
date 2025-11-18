using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Chairman")]
    public class SectorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List of sectors
        public IActionResult Index()
        {
            var sectors = _context.Sectors.ToList();
            return View(sectors);
        }

        // GET: /Admin/Sector/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Sector/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sector model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Sectors.Add(model);
            _context.SaveChanges();

            TempData["success"] = "تم إضافة القطاع بنجاح";
            return RedirectToAction("Index");
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var sector = _context.Sectors.FirstOrDefault(s => s.Id == id);
            if (sector == null)
                return NotFound();

            return View(sector);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Sector model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Sectors.Update(model);
            _context.SaveChanges();

            TempData["success"] = "تم تعديل القطاع بنجاح";
            return RedirectToAction("Index");
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var sector = _context.Sectors.FirstOrDefault(s => s.Id == id);
            if (sector == null)
                return NotFound();

            return View(sector);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var sector = _context.Sectors.FirstOrDefault(s => s.Id == id);
            if (sector == null)
                return NotFound();

            _context.Sectors.Remove(sector);
            _context.SaveChanges();

            TempData["success"] = "تم حذف القطاع بنجاح";
            return RedirectToAction("Index");
        }
    }
}
