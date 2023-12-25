using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class TrackRecordMetrics
    {
        public double RequiredMonthlyDays { get; set; }
        public double TrackTimeCurrentMonth { get; set; }
        public double OvertimeTrackTimeCurrentMonth { get; set; }
    }
}
