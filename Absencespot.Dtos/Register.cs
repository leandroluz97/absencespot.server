using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Register
    {
        public string Email { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentException(nameof(Email));
            }
        }
    }
}
