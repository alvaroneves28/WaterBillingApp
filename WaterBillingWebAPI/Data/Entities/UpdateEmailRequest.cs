namespace WaterBillingWebAPI.Data.Entities
{
    public class UpdateEmailRequest
    {
        public string NewEmail { get; set; }
        public string CurrentPassword { get; set; }
    }
}
