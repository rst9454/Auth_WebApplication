using Auth_WebApplication.Repostory.Interface;
using Auth_WebApplication.ViewModels.Email;
using System.Net;
using System.Net.Mail;

namespace Auth_WebApplication.Repostory.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> EmailSendAsync(string email, string Subject, string message)
        {
            bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKey = configuration.GetValue<string>("AppSettings:SecretKey"),
                    From = configuration.GetValue<string>("AppSettings:EmailSettings:From"),
                    SmtpServer = configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer"),
                    Port = configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnablSSL"),
                };
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject = Subject,
                    Body = message,
                    BodyEncoding = System.Text.Encoding.ASCII,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);
                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new NetworkCredential(getEmailSetting.From, getEmailSetting.SecretKey),
                    EnableSsl = getEmailSetting.EnableSSL
                };
                await smtpClient.SendMailAsync(mailMessage);
                status = true;

            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }
    }
}
