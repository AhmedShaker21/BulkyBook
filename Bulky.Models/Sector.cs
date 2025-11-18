using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Sector
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // مدير القطاع
        public string? SectorManagerId { get; set; }

        // المدير العام (اختياري)
        public string? GeneralManagerId { get; set; }
    }
}
