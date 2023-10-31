using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Absence : Entity
    {
        public double Allowance { get; set; }
        public double MonthlyAccrual { get; set; }
        public double MonthCarryOverExpiresAfter { get; set; }
        public double MonthMaxCarryOver { get; set; }

        public int? OfficeId { get; set; }
        public Office? Office { get; set; }
        public int? LeaveId { get; set; }
        public Leave? Leave { get; set; }
    }
}
