using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Clients.GoogleCalendar.Options
{
    public class GoogleAuthOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Scopes { get; set; } = null!;
        public string User { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public string KeyFilePath { get; set; } = null!;

        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
    }
}
