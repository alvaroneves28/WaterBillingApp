namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Represents SMTP configuration settings for sending emails.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the SMTP server host name or IP address.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP server port number.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for the SMTP connection.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets the email address used as the sender.
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name used as the sender.
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username used for SMTP authentication.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password used for SMTP authentication.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
