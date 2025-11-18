using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public enum MemoStatus
    {
        New = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Closed = 4
    }
    public class Memo
    {
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        public int FromDepartmentId { get; set; }

        [ValidateNever]
        public Department? FromDepartment { get; set; }

        public int ToDepartmentId { get; set; }

        [ValidateNever]
        public Department? ToDepartment { get; set; }

        public string CreatedByUserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public MemoStatus Status { get; set; } = MemoStatus.New;

        public string? ReviewedByUserId { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? RejectedByUserId { get; set; }
        public string? ClosedByUserId { get; set; }
    }

}
