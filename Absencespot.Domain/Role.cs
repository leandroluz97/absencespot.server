using Absencespot.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Absencespot.Domain
{
    public class Role : IdentityRole<Guid>
    {
        public RoleTypes Name { get; set; }
        public string NormalizedName
        {
            get
            {
                return Name.DisplayName;
            }
        }
    }
}
