namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents a digital signature associated with an invoice.
    /// </summary>
    public class InvoiceSignature
    {
        /// <summary>
        /// The ID of the invoice that this signature belongs to.
        /// Acts as a foreign key.
        /// </summary>
        public int InvoiceId { get; set; }

        /// <summary>
        /// The digital signature stored as a Base64-encoded string.
        /// </summary>
        public string AssinaturaBase64 { get; set; }
    }
}
