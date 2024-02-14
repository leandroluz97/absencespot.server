using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares.JwtAuthentication
{
    public class JwtAuthenticationOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string SecretKey { get; set; }
        public int Expirationtime { get; set; }
    }
}
