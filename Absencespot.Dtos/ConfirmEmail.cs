using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class ConfirmEmail
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }


        public void EnsureValidation()
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new ArgumentException(nameof(Token));
            }
            if (string.IsNullOrEmpty(Email))
            {
                throw new ArgumentException(nameof(Email));
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                throw new ArgumentException(nameof(FirstName));
            }
            if (string.IsNullOrEmpty(LastName))
            {
                throw new ArgumentException(nameof(LastName));
            }
            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException(nameof(Password));
            }
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                throw new ArgumentException(nameof(PhoneNumber));
            }
        }
    }
}
