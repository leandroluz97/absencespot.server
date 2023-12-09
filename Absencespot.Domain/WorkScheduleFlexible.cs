using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class WorkScheduleFlexible : WorkSchedule
    {
        public int? TotalWorkDays { get; set; }
        public int? Hours { get; set; }
    }
}
