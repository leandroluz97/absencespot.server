using Absencespot.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Absencespot.Domain
{
    public class Role : IdentityRole<int>
    {
        public Guid GlobalId { get; set; }
        private RoleType _type { get; set; }
        public override string Name {
            get
            {
                return _type.Name;
            }
            set
            {
                _type =  RoleType.FromDisplayName<RoleType>(value);
            } 
        }
        public string NormalizedName
        {
            get
            {
                return Name;
            }
        }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
