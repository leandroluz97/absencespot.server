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
    }
}
