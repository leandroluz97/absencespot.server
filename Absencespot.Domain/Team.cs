using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Team : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAutoApprove { get; set; }
        public ICollection<User> Managers { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
