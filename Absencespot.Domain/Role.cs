using Absencespot.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Absencespot.Domain
{
    public class Role : IdentityRole<int>
    {
        public Guid GlobalId { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get;}

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
