using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;

namespace Absencespot.Domain
{
    public class Role : Entity
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
