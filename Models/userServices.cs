using Microsoft.AspNetCore.Identity;
using SendingOTP.services;


namespace SendingOTP.Model
{
    public class userService
    {
        private readonly IEmailservice _emailService;

        public userService(IEmailservice emailService)
        {
            _emailService = emailService;
        }

        private string GenerateNumber()
        {
            Random random = new Random();
            string randomNumber = random.Next(0, 1000000).ToString("D6");
            return randomNumber;
        }

        private async Task SendOtpMail(string userEmail, string otpText, string name)
        {
            var mailRequest = new MailRequest
            {
                Email = userEmail,
                Subject = "Thanks for registering: OTP",
                Emailbody = GenerateEmailBody(name, otpText)
            };
            await _emailService.SendEmail(mailRequest);
        }

        private string GenerateEmailBody(string name, string otpText)
        {
            return $@"
                <div>
                    <h1>Hi {name}, Thanks for registering</h1>
                    <h2>Please enter the OTP and complete the registration</h2>
                    <h2>OTP Text is: {otpText}</h2>
                </div>";
        }
    }
}
