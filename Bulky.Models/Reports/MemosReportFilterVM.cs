using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Reports
{
    public class MemosReportFilterVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? SectorId { get; set; }
        public int? DepartmentId { get; set; }
        public MemoStatus? Status { get; set; }

        public IEnumerable<Sector>? Sectors { get; set; }
        public IEnumerable<Department>? Departments { get; set; }
        public IEnumerable<Memo>? Result { get; set; }
    }
}
