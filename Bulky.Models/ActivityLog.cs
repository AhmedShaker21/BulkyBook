using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int MemoId { get; set; }
        public Memo Memo { get; set; } = null!;

        public string Action { get; set; } = null!; // "Created", "Approved", "Rejected", "Closed"...
        public MemoStatus? OldStatus { get; set; }
        public MemoStatus? NewStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Note { get; set; }
    }
}
