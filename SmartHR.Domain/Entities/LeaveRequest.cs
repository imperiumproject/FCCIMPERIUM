using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Domain.Entities
{
    public class LeaveRequest
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Status { get; private set; }

        public LeaveRequest(Guid id, Guid employeeId, DateTime startDate, DateTime endDate, string status)
        {
            Id = id;
            EmployeeId = employeeId;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
        }
    }
}
