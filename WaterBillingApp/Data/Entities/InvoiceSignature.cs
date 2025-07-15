namespace WaterBillingApp.Data.Entities
{
    public class InvoiceSignature
    {
        public int InvoiceId { get; set; }
        public string AssinaturaBase64 { get; set; }
    }
}
