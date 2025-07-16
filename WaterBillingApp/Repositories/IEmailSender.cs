namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Defines a contract for sending emails asynchronously.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body content of the email, typically in HTML format.</param>
        /// <returns>A task representing the asynchronous send operation.</returns>
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
