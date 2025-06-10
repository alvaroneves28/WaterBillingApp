
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace REMOVED.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

     
        public EmailSender(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var mail = new MailMessage()
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = REMOVED,
                };
                mail.To.Add(toEmail);

                using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl,
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Log ou output para debug
                Console.WriteLine("Erro a enviar email: " + ex.Message);
                throw; // opcional, para deixar o erro propagar
            }
        }

    }
}
