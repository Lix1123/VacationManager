using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using VacationManager.Models;
using VacationManager.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VacationManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string? selectedUserId = null, LeaveStatus? selectedStatus = null)
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.User)
                .ToListAsync();

            if (!string.IsNullOrEmpty(selectedUserId))
            {
                leaveRequests = leaveRequests.Where(lr => lr.UserId == selectedUserId).ToList();
            }

            if (selectedStatus.HasValue)
            {
                leaveRequests = leaveRequests.Where(lr => lr.Status == selectedStatus.Value).ToList();
            }

            var projects = await _context.Projects.ToListAsync();
            var logs = await _context.RequestLogs
                .Include(l => l.LeaveRequest)
                .ThenInclude(r => r.User)
                .Include(l => l.PerformedBy)
                .ToListAsync();

            var model = new DashboardViewModel(_userManager)
            {
                Users = users,
                Roles = roles,
                LeaveRequests = leaveRequests,
                Projects = projects,
                RequestLogs = logs,
                SelectedUserId = selectedUserId,
                SelectedStatus = selectedStatus
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveLeave(int leaveRequestId)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.User)
                .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest != null && leaveRequest.Status == LeaveStatus.Pending)
            {
                leaveRequest.Status = LeaveStatus.Approved;
                _context.LeaveRequests.Update(leaveRequest);

                // 🔔 Известие към потребителя
                _context.Notifications.Add(new Notification
                {
                    RecipientId = leaveRequest.UserId,
                    Message = $"Заявката ви за отпуск от {leaveRequest.StartDate:dd.MM.yyyy} до {leaveRequest.EndDate:dd.MM.yyyy} беше <b>одобрена</b>.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

                // 🕓 Добавяне в лог
                var currentUser = await _userManager.GetUserAsync(User);
                _context.RequestLogs.Add(new RequestLog
                {
                    LeaveRequestId = leaveRequest.Id,
                    PerformedById = currentUser.Id,
                    Action = "Одобрена",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectLeave(int leaveRequestId)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.User)
                .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest != null && leaveRequest.Status == LeaveStatus.Pending)
            {
                leaveRequest.Status = LeaveStatus.Rejected;

                var user = leaveRequest.User;
                if (user != null)
                {
                    int days = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1;
                    user.LeaveDaysRemaining += days;
                    _context.Users.Update(user);

                    // 🔔 Известие към потребителя
                    _context.Notifications.Add(new Notification
                    {
                        RecipientId = user.Id,
                        Message = $"Заявката ви за отпуск от {leaveRequest.StartDate:dd.MM.yyyy} до {leaveRequest.EndDate:dd.MM.yyyy} беше <b>отказана</b>.",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }

                _context.LeaveRequests.Update(leaveRequest);

                // 🕓 Добавяне в лог
                var currentUser = await _userManager.GetUserAsync(User);
                _context.RequestLogs.Add(new RequestLog
                {
                    LeaveRequestId = leaveRequest.Id,
                    PerformedById = currentUser.Id,
                    Action = "Отказана",
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
