using BulkyBook.DataAcess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Chairman, SectorManager, DepartmentManager, GeneralManager")]
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Tree()
        {
            var sectors = _context.Sectors
                .Select(s => new OrganizationTreeViewModel
                {
                    SectorId = s.Id,
                    SectorName = s.Name,

                    Departments = _context.Departments
                        .Where(d => d.SectorId == s.Id)
                        .Select(d => new DepartmentNode
                        {
                            DepartmentId = d.Id,
                            DepartmentName = d.Name,

                            Users = _context.Users
                                .Where(u => u.DepartmentId == d.Id)
                                .Select(u => new UserNode
                                {
                                    Name = u.Name,
                                    Role = _context.UserRoles
                                        .Where(ur => ur.UserId == u.Id)
                                        .Select(ur => _context.Roles.First(r => r.Id == ur.RoleId).Name)
                                        .FirstOrDefault()
                                }).ToList()
                        }).ToList()
                })
                .ToList();

            return View(sectors);
        }
    }

    // View Models
    public class OrganizationTreeViewModel
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public List<DepartmentNode> Departments { get; set; }
    }

    public class DepartmentNode
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<UserNode> Users { get; set; }
    }

    public class UserNode
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
