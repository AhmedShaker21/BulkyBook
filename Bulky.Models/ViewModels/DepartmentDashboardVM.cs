using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels
{
    public class DepartmentDashboardVM
    {
        public string DepartmentName { get; set; }
        public int EmployeesCount { get; set; }
        public int Inbox { get; set; }
        public int Outbox { get; set; }
        public int ReviewCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<Memo> LatestMemos { get; set; }
    }

}
