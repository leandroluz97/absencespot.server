using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int OfficeId { get; set; }
        public Office Office { get; set; }
    }
}
