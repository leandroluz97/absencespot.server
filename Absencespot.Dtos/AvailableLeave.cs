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
        public int? AbsenceId { get; set; }
        public Absence? Absence { get; set; }

    }
}
