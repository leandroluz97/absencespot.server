using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface IEmailOptions
    {
        string SenderEmail { get; set; }
        string ReceiverEmail { get; set; }
        string TemplateId { get; set; }
        Dictionary<string, string> TemplateData { get; set; }
    }
}
