using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Leave : Entity
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsReasonRequired { get; set; }
        public bool IsLimitedQuota { get; set; }
        public decimal? YearlyQuota { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
