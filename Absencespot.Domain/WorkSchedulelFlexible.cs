using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class WorkSchedulelFlexible : WorkSchedule
    {
        public int WorkDays { get; set; }
        public int Hours { get; set; }
    }
}
