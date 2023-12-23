using Absencespot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class RejectRequest
    {
        public StatusType Status { get; set; }
        public Guid ApproverId { get; set; }
        public string Reason { get; set; }

        public void EnsureValidation()
        {
            if (Status != StatusType.Rejected)
            {
                throw new ArgumentException(nameof(Status));
            }
            if (ApproverId == default)
            {
                throw new ArgumentException(nameof(ApproverId));
            }
        }
    }
}
