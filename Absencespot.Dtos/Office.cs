using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Office
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsEmployeeLeaveStartOnJobStartDate { get; set; }
        public Address Address { get; set; }
        public IEnumerable<Guid>? AvailableLeaves { get; set; }
        public IEnumerable<Absence>? Absences { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
            if (Absences == null || !Absences.Any())
            {
                throw new ArgumentException(nameof(Absences));
            }
            if (AvailableLeaves == null || !AvailableLeaves.Any())
            {
                throw new ArgumentException(nameof(AvailableLeaves));
            }
            if (Absences == null || !Absences.Any())
            {
                throw new ArgumentException(nameof(Absences));
            }
        }
    }
}
