using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Leave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsReasonRequired { get; set; }
        public bool IsLimitedQuota { get; set; }
        public decimal? YearlyQuota { get; set; }
        public IEnumerable<Guid> OfficeIds { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
            if (string.IsNullOrWhiteSpace(Icon))
            {
                throw new ArgumentException(nameof(Icon));
            }
            if (OfficeIds == null || !OfficeIds.Any())
            {
                throw new ArgumentException(nameof(OfficeIds));
            }
        }
    }
}
