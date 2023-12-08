using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class WorkScheduleDefault : WorkSchedule
    {
        //default
        public IEnumerable<string> WorkDays { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime EndHour { get; set; }

        public override void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
        }
    }
}
