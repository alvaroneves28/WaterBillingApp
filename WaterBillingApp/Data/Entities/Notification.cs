namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents a notification message sent to a customer or employee.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Primary key of the notification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The content of the notification message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether the notification has been read by the recipient.
        /// Defaults to false (unread).
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date and time when the notification was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Optional foreign key to the customer who receives the notification.
        /// Null if the notification is not related to a specific customer.
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Navigation property to the related customer.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Flag indicating if the notification is intended for an employee instead of a customer.
        /// Defaults to false.
        /// </summary>
        public bool ForEmployee { get; set; } = false;
    }
}
