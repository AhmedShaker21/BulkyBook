using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ============================
        // عرض كل المستخدمين
        // ============================
        public IActionResult Index()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    Role = _context.UserRoles
                            .Where(r => r.UserId == u.Id)
                            .Select(r => _context.Roles.FirstOrDefault(x => x.Id == r.RoleId).Name)
                            .FirstOrDefault(),
                    Sector = u.SectorId != null ? _context.Sectors.FirstOrDefault(s => s.Id == u.SectorId).Name : "-",
                    Department = u.DepartmentId != null ? _context.Departments.FirstOrDefault(d => d.Id == u.DepartmentId).Name : "-"
                })
                .ToList();

            return View(users);
        }

        // ============================
        // GET: Create
        // ============================
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name");
            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "Id", "Name");

            return View(new UserCreateViewModel());
        }

        // ============================
        // POST: Create
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name");
                ViewBag.Departments = new SelectList(_context.Departments.ToList(), "Id", "Name");
                return View(model);
            }

            var user = new ApplicationUser
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,  // مهم جداً
                SectorId = model.SectorId,
                DepartmentId = model.DepartmentId
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["success"] = "تم إنشاء المستخدم بنجاح!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ============================
        // GET: Edit
        // ============================
        public IActionResult Edit(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name",
                _userManager.GetRolesAsync(user).Result.FirstOrDefault());

            ViewBag.Sectors = new SelectList(_context.Sectors.ToList(), "Id", "Name", user.SectorId);
            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "Id", "Name", user.DepartmentId);

            return View(user);
        }

        // ============================
        // POST: Edit
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;   // مهم
            user.SectorId = model.SectorId;
            user.DepartmentId = model.DepartmentId;

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles.ToArray());
            await _userManager.AddToRoleAsync(user, role);

            _context.SaveChanges();

            TempData["success"] = "تم تحديث المستخدم بنجاح!";
            return RedirectToAction("Index");
        }

        // ============================
        // GET: Delete
        // ============================
        public IActionResult Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ============================
        // POST: Delete Confirm
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["success"] = "تم حذف المستخدم بنجاح!";
            return RedirectToAction("Index");
        }
    }
}
