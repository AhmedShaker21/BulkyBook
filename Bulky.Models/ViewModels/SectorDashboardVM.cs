using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels
{
    public class SectorDashboardVM
    {
        public string SectorName { get; set; }
        public List<Department> Departments { get; set; }
        public int EmployeesCount { get; set; }

        public int SentMemos { get; set; }
        public int ReceivedMemos { get; set; }

        public List<Memo> LatestMemos { get; set; }
    }
}
