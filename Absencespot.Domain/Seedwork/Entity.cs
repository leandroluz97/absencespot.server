using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Seedwork
{
    public abstract class Entity : Signature
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
    }
}
