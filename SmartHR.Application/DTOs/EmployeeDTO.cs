using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Application.DTOs
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
    }
}
