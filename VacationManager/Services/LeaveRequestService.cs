using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using VacationManager.Models;

namespace VacationManager.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task CreateLeaveRequestAsync(LeaveRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                Console.WriteLine("❌ CreateLeaveRequestAsync: UserId е празен!");
                return;
            }

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Заявката е запазена успешно.");
        }

        public async Task UpdateLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveRequestAsync(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request != null)
            {
                _context.LeaveRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
