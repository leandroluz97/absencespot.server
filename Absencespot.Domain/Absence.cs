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
        public decimal Allowance { get; set; }
        public decimal MonthlyAccrual { get; set; }
        public decimal MonthCarryOverExpiresAfter { get; set; }
        public decimal MonthMaxCarryOver { get; set; }

        public int OfficeId { get; set; }
        public Office Office { get; set; }
        public int LeaveId { get; set; }
        public Leave Leave { get; set; }
    }
}
