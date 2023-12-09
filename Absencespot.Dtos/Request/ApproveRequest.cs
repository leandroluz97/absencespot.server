using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class ApproveRequest
    {
        public int Status { get; set; }
        public Guid ApproverId { get; set; }
    }
}
