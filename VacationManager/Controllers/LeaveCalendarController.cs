using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using System.Linq;

namespace VacationManager.Controllers
{
    [Authorize]
    public class LeaveCalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveCalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaveEvents()
        {
            var events = await _context.LeaveRequests
                .Include(lr => lr.User)
                .Select(lr => new
                {
                    title = lr.User.UserName + ": " + lr.Reason,
                    start = lr.StartDate.ToString("yyyy-MM-dd"),
                    end = lr.EndDate.AddDays(1).ToString("yyyy-MM-dd"),
                    color = lr.Status == Models.LeaveStatus.Approved ? "#28a745" :
                            lr.Status == Models.LeaveStatus.Rejected ? "#dc3545" : "#ffc107"
                })
                .ToListAsync();

            return Json(events);
        }
    }
}
