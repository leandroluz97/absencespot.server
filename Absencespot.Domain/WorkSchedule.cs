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

        public int CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
