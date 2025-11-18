using BulkyBook.Models;
using System.ComponentModel.DataAnnotations;
namespace BulkyBook.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]    // يضمن إن الفاليديشن يحصل
        public int SectorId { get; set; }

        public Sector Sector { get; set; }
    }
}