using Absencespot.Infrastructure.Abstractions.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Clients.Sendgrid
{
    public class SendgridOptions : IEmailOptions
    {
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string TemplateId { get; set; }
        public Dictionary<string, string> TemplateData { get; set; }
    }
}
