using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Project
    {
        public string Code { get;  init; }
        public string Name { get;  init; }
        public decimal ProjectCost { get;  init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public List<Employee> Employees { get;  init; } 
        public List<Owner> Owners { get; init; }
    }
}
