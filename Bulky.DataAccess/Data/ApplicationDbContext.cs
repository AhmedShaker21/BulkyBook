using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BulkyBook.DataAcess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Memo>()
                .HasOne(m => m.FromDepartment)
                .WithMany()
                .HasForeignKey(m => m.FromDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Memo>()
                .HasOne(m => m.ToDepartment)
                .WithMany()
                .HasForeignKey(m => m.ToDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Memo> Memos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }


    }
}
