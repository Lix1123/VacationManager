using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VacationManager.Models
{
    public class DashboardViewModel
    {
        private readonly UserManager<User> _userManager;

        public DashboardViewModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public List<User> Users { get; set; } = new List<User>();
        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
        public List<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<RequestLog> RequestLogs { get; set; } = new();

        // 🔍 Филтри
        public string? SelectedUserId { get; set; }
        public LeaveStatus? SelectedStatus { get; set; }

        // 🔄 Метрики
        public int TotalUsers => Users.Count;
        public int TotalRequests => LeaveRequests.Count;
        public int ApprovedRequests => LeaveRequests.Count(r => r.Status == LeaveStatus.Approved);
        public int RejectedRequests => LeaveRequests.Count(r => r.Status == LeaveStatus.Rejected);
        public int PendingRequests => LeaveRequests.Count(r => r.Status == LeaveStatus.Pending);

        // 🔐 Взимане на роля по потребител
        public async Task<string?> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault() ?? "Няма роля";
            }
            return "Няма роля";
        }
    }
}
