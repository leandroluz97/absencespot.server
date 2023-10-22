using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class StatusType : Enumeration
    {
        public static readonly StatusType APPROVED = new StatusType(1, "APPROVED");
        public static readonly StatusType PENDING = new StatusType(2, "PENDING");
        public static readonly StatusType REJECTED = new StatusType(3, "REJECTED");

        public StatusType(int id, string name) : base(id, name)
        {

        }
    }
}
