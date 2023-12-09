using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class WorkSchedule : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

        //default
        public IEnumerable<string>? WorkDays { get; set; }
        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }

        //flexible
        public int? TotalWorkDays { get; set; }
        public int? Hours { get; set; }


        public int CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
