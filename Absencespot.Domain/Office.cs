using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Office : Entity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsEmployeeLeaveStartOnJobStartDate { get; set; }
        public Address Address { get; set; }
        //public ICollection<OfficeLeave>? AvailableLeaves { get; set; }
        public ICollection<Absence>? Absences { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }


    }
}
