using SmartHR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Domain.Interfaces
{
    public interface ID365ApiClient
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsAsync();
        Task<IEnumerable<MenuItem>> GetMenuItemsFromD365Async();

    }
}
