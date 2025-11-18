using System;
using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels
{
    public class DashboardVM
    {
        public int SectorsCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int UsersCount { get; set; }
        public int MemosCount { get; set; }

        public int NewMemos { get; set; }
        public int UnderReview { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Closed { get; set; }

        public List<Memo> LatestMemos { get; set; }
        public List<MonthlyMemoStat> MonthlyStats { get; set; }

    }

    public class MonthlyMemoStat
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }
}
