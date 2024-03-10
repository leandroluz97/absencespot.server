using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class AuthorizationCode
    {
        public string Code { get; set; }
        public string? State { get; set; }
        public string? CodeVerifier { get; set; }
        public string? IdentityProvider { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                throw new ArgumentException("Code is required.", nameof(Code));
            }
        }
    }
}
