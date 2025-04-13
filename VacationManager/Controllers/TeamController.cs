using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VacationManager.Data;
using VacationManager.Models;

namespace VacationManager.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TeamController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams
                .Include(t => t.TeamLead)
                .Include(t => t.Project)
                .ToListAsync();
            return View(teams);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", team.TeamLeadId);
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name", team.ProjectId);
            return View(team);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", team.TeamLeadId);
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name", team.ProjectId);
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Team team)
        {
            if (id != team.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", team.TeamLeadId);
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name", team.ProjectId);
            return View(team);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var team = await _context.Teams
                .Include(t => t.TeamLead)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
