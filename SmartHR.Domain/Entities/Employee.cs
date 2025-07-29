using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHR.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Department { get; private set; }
        public string Email { get; private set; }

        public Employee(Guid id, string fullName, string department, string email)
        {
            Id = id;
            FullName = fullName;
            Department = department;
            Email = email;
        }
    }
}
