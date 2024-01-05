using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class AvailableLeave
    {
        public double AvailableDays { get; set; }
        public DateTime Period { get; set; }
        public Guid AbsenceId { get; set; }
        public Guid? LeaveId { get; set; }

    }
}
