namespace WaterBillingApp.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Relação com o Cliente
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public bool ForEmployee { get; set; } = false;

    }
}
