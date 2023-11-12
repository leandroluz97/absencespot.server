using Absencespot.Clients.Sendgrid;
using Absencespot.Infrastructure.Abstractions.Clients;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Absencespot.Clients.Sendgrid
{
    public class SendridClient : IEmailClient
    {
        private readonly IConfiguration _configuration;
        public SendridClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(IEmailOptions sendEmailDto)
        {
            var client = new SendGridClient(_configuration.GetConnectionString("SendGrid"));
            var sendGridMessage = new SendGridMessage();

            sendGridMessage.SetFrom(new EmailAddress(sendEmailDto.SenderEmail));
            sendGridMessage.AddTo(new EmailAddress(sendEmailDto.ReceiverEmail));
            sendGridMessage.SetTemplateId(sendEmailDto.TemplateId);
            sendGridMessage.SetTemplateData(sendEmailDto.TemplateData);

            var response = await client.SendEmailAsync(sendGridMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send confirmation email.StatusCode ={ response.StatusCode }, Body ={ await response.Body.ReadAsStringAsync()}");
            }
        }

    }
}