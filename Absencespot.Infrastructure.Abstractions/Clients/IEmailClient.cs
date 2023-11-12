using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface IEmailClient
    {
        Task SendEmailAsync(IEmailOptions options);
    }
}
