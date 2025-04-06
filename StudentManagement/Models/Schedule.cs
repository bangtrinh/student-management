using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentManagement.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [Required]
        [StringLength(10)]
        public string StudentID { get; set; }

        [Required]
        [StringLength(10)]
        public string CourseID { get; set; }

        [Required]
        public DateTime ClassDate { get; set; }  // Ngày học

        [Required]
        public TimeSpan StartTime { get; set; }  // Giờ bắt đầu

        [Required]
        public TimeSpan EndTime { get; set; }    // Giờ kết thúc

        // Navigation properties
        [ValidateNever]
        public virtual Student Student { get; set; }
        [ValidateNever]
        public virtual Course Course { get; set; }
    }
}
