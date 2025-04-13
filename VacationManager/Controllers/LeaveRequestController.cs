using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VacationManager.Models;
using VacationManager.Services;
using VacationManager.Data;
using System;

namespace VacationManager.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public LeaveRequestController(
            ILeaveRequestService leaveRequestService,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _leaveRequestService = leaveRequestService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isTeamLead = await _userManager.IsInRoleAsync(user, "TeamLead");

            var allRequests = await _leaveRequestService.GetAllLeaveRequestsAsync();

            var visibleRequests = (isAdmin || isTeamLead)
                ? allRequests
                : allRequests.Where(r => r.UserId == user.Id);

            return View(visibleRequests);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Не сте влезли в системата.");
                return View(model);
            }

            int requestedDays = (model.EndDate - model.StartDate).Days + 1;
            if (requestedDays <= 0)
            {
                ModelState.AddModelError("", "Крайната дата трябва да е след началната.");
                return View(model);
            }

            if (user.LeaveDaysRemaining < requestedDays)
            {
                ModelState.AddModelError("", $"Нямате достатъчно дни отпуск. Оставащи: {user.LeaveDaysRemaining}");
                return View(model);
            }

            user.LeaveDaysRemaining -= requestedDays;
            model.UserId = user.Id;
            model.Status = LeaveStatus.Pending;
            model.CreatedAt = DateTime.UtcNow;

            await _leaveRequestService.CreateLeaveRequestAsync(model);
            await _userManager.UpdateAsync(user);

            var teamLeads = await _userManager.GetUsersInRoleAsync("TeamLead");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var recipients = teamLeads.Concat(admins).Distinct();

            foreach (var recipient in recipients)
            {
                _context.Notifications.Add(new Notification
                {
                    RecipientId = recipient.Id,
                    Message = $"Нова заявка за отпуск от {user.FirstName} {user.LastName}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            _context.RequestLogs.Add(new RequestLog
            {
                LeaveRequestId = model.Id,
                Action = "Създадена заявка",
                PerformedById = user.Id,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
            if (request == null) return NotFound();

            request.Status = LeaveStatus.Approved;
            await _leaveRequestService.UpdateLeaveRequestAsync(request);

            var currentUser = await _userManager.GetUserAsync(User);

            _context.Notifications.Add(new Notification
            {
                RecipientId = request.UserId,
                Message = $"Заявката ви за отпуск от {request.StartDate:dd.MM.yyyy} до {request.EndDate:dd.MM.yyyy} е одобрена.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            _context.RequestLogs.Add(new RequestLog
            {
                LeaveRequestId = request.Id,
                Action = "Одобрена заявка",
                PerformedById = currentUser.Id,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
            if (request == null) return NotFound();

            request.Status = LeaveStatus.Rejected;

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user != null)
            {
                int days = (request.EndDate - request.StartDate).Days + 1;
                user.LeaveDaysRemaining += days;
                await _userManager.UpdateAsync(user);
            }

            await _leaveRequestService.UpdateLeaveRequestAsync(request);

            var currentUser = await _userManager.GetUserAsync(User);

            _context.Notifications.Add(new Notification
            {
                RecipientId = request.UserId,
                Message = $"Заявката ви за отпуск от {request.StartDate:dd.MM.yyyy} до {request.EndDate:dd.MM.yyyy} беше отказана.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            _context.RequestLogs.Add(new RequestLog
            {
                LeaveRequestId = request.Id,
                Action = "Отказана заявка",
                PerformedById = currentUser.Id,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var allRequests = _context.LeaveRequests.ToList();

            if (allRequests.Any())
            {
                _context.LeaveRequests.RemoveRange(allRequests);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Всички заявки са успешно изтрити.";
            }
            else
            {
                TempData["Message"] = "Няма заявки за изтриване.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
