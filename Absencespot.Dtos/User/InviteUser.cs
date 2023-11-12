using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class InviteUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string RoleId { get; set; }
        public Guid WorkScheduleId { get; set; }
        public IEnumerable<Guid> TeamIds { get; set; }
        public Guid OfficeId { get; set; }
        public DateTime StartedWorkingDay { get; set; }
        public DateTime? BirthDay { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                throw new ArgumentException(nameof(FirstName));
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                throw new ArgumentException(nameof(LastName));
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentException(nameof(Email));
            }
            if (string.IsNullOrWhiteSpace(Position))
            {
                throw new ArgumentException(nameof(Position));
            }
            if (WorkScheduleId == default)
            {
                throw new ArgumentException(nameof(WorkScheduleId));
            }
            if (OfficeId == default)
            {
                throw new ArgumentException(nameof(OfficeId));
            }
            if (!TeamIds.Any())
            {
                throw new ArgumentException(nameof(OfficeId));
            }
        }
    }
}
