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
    public class EmployeeService : IEmployeeService
    {
        private readonly ID365ApiClient _d365ApiClient;

        public EmployeeService(ID365ApiClient d365ApiClient)
        {
            _d365ApiClient = d365ApiClient;
        }
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _d365ApiClient.GetEmployeesAsync();
            return employees.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Department = e.Department,
                Email = e.Email
            });
        }
    }
}
