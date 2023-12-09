using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class WorkScheduleFlexible : WorkSchedule
    {
        public int? WorkDays { get; set; }
        public int? Hours { get; set; }

        public override void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
        }
    }
}
