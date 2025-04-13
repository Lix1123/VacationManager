using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VacationManager.Data;
using VacationManager.Models;

public class NotificationViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public NotificationViewComponent(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
        if (user == null) return View(new List<Notification>());

        var notifications = await _context.Notifications
            .Where(n => n.RecipientId == user.Id && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        return View(notifications);
    }
}
