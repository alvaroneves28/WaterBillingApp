namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for displaying error information.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The ID of the current request, useful for tracking errors.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether to show the RequestId (if it is not null or empty).
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// The error message to be displayed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
