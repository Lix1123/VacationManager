using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManager.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

     
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Начална дата")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Крайна дата")]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Причина")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Статус")]
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        [Display(Name = "Дата на създаване")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
