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
        public bool IsAutoApproved { get; set; }
        public ICollection<UserTeam> Managers { get; set; }
        public ICollection<UserTeam> Users { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
