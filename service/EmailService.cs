using Microsoft.Extensions.Options;
using SendingOTP.Model;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using SendingOTP.services;

namespace SendingOTP.Container
{
    public class EmailService : IEmailservice
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendEmail(MailRequest request)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(request.Email));
            email.Subject = request.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = request.Emailbody
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email", ex);
            }
            finally
            {
                smtp.Disconnect(true);
                smtp.Dispose();
            }
        }
    }
}
