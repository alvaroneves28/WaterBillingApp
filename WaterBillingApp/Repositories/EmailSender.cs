using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Provides functionality to send emails using SMTP.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class with specified SMTP settings.
        /// </summary>
        /// <param name="smtpSettings">The SMTP configuration settings.</param>
        public EmailSender(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="toEmail">The recipient email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="htmlMessage">The HTML body content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws if sending email fails.</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var mail = new MailMessage()
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
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
                Console.WriteLine("Error sending email: " + ex.Message);
                throw;
            }
        }
    }
}
