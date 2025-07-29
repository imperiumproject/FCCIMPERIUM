using SmartHR.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveDTO>> GetAllLeaveRequestsAsync();
    }
}
