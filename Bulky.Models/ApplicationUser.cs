using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models {
	public class ApplicationUser:IdentityUser {
		[Required]
        public string? Name { get; set; }

        // ربطه بسكتور وإدارة
        public int? SectorId { get; set; }
        public Sector? Sector { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
