using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels
{
    public class EmployeeDashboardVM
    {
        public int Sent { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }

        public List<Memo> LatestMemos { get; set; }
    }
}
