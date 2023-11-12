using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                throw new ArgumentException(nameof(CurrentPassword));
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                throw new ArgumentException(nameof(NewPassword));
            }
        }
    }
}
