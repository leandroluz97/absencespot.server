using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? ProfileUrl { get; set; }

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
        }
    }
}
