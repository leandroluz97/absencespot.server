using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class ResetPassword
    {
        public string Email { get; set; }
        public string ActivationToken { get; set; }
        public string Password { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentException(nameof(Email));
            }
            if (string.IsNullOrWhiteSpace(ActivationToken))
            {
                throw new ArgumentException(nameof(ActivationToken));
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException(nameof(Password));
            }
        }
    }
}
