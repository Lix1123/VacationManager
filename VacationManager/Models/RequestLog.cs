using System;

namespace VacationManager.Models
{
    public class RequestLog
    {
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public LeaveRequest? LeaveRequest { get; set; }

        public string Action { get; set; } = string.Empty; // "Одобрена", "Отказана", "Създадена"
        public string PerformedById { get; set; } = string.Empty;
        public User? PerformedBy { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
