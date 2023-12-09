using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class WorkScheduleDefault : WorkSchedule
    {
        public IEnumerable<string>? WorkDays { get; set; }
        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }
    }
}
