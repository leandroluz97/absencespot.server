using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class OfficeLeave : Entity
    {
        public int? OfficeId { get; set; }
        public Office? Office { get; set; }
        public int? LeaveId { get; set; }
        public Leave? Leave { get; set; }
    }
}
