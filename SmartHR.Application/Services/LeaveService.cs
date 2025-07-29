using SmartHR.Application.DTOs;
using SmartHR.Application.Interfaces;
using SmartHR.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ID365ApiClient _d365ApiClient;

        public LeaveService(ID365ApiClient d365ApiClient)
        {
            _d365ApiClient = d365ApiClient;
        }
        public async Task<IEnumerable<LeaveDTO>> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await _d365ApiClient.GetLeaveRequestsAsync();
            return leaveRequests.Select(lr => new LeaveDTO
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Status = lr.Status
            });
        }
    }
}
