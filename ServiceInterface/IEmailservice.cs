using SendingOTP.Model;

namespace SendingOTP.services
{
    public interface IEmailservice
    {
        Task SendEmail(MailRequest request);
    }
}