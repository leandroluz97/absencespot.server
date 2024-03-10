using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class MicrosoftAuthOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Scopes { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;
        public string AuthorityUri { get; set; } = null!;
    }
}
