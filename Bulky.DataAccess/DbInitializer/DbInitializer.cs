using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer {
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch { }

            // Roles
            if (!_roleManager.RoleExistsAsync("Chairman").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Chairman")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("SectorManager")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("GeneralManager")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("DepartmentManager")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Employee")).GetAwaiter().GetResult();

                // Create Chairman user
                var user = new ApplicationUser
                {
                    UserName = "chairman@org.com",
                    Email = "chairman@org.com",
                    Name = "Chairman",
                    EmailConfirmed = true
                };

                _userManager.CreateAsync(user, "Admin123!").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "Chairman").GetAwaiter().GetResult();
            }
        }
    }
}
