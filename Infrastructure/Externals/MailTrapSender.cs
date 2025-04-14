using Core.ServicesContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Externals
{
    public class MailTrapSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MailTrapSender> _logger;

        public MailTrapSender(IConfiguration config, ILogger<MailTrapSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void SendEmail(string from, string to, string sub, string body)
        {
            var smtp = _config.GetSection("SMTP");
            string host = smtp["Host"]!;
            string port = smtp["Port"]!;
            string user = smtp["Username"]!;
            string pass = smtp["Password"]!;

            var client = new SmtpClient(host, int.Parse(port))
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };
            client.Send(from, to, sub, body);
            _logger.LogInformation($"Email sent from '{from}' to '{to}'");
        }
    }
}
