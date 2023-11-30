using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Absence
    {
        public double Allowance { get; set; }
        public double MonthlyAccrual { get; set; }
        public double MonthCarryOverExpiresAfter { get; set; }
        public double MonthMaxCarryOver { get; set; }
        public Guid LeaveId { get; set; }
    }
}
