using System.Collections.Generic;
using System.Threading.Tasks;
using VacationManager.Models;

namespace VacationManager.Services
{
    public interface ILeaveRequestService
    {
        // Взима всички заявки за отпуска (включително с потребителите)
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync();

        // Взима една заявка по Id
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);

        // Създава нова заявка
        Task CreateLeaveRequestAsync(LeaveRequest request);

        // Актуализира съществуваща заявка
        Task UpdateLeaveRequestAsync(LeaveRequest request);

        // Изтрива заявка по Id
        Task DeleteLeaveRequestAsync(int id);
    }
}
