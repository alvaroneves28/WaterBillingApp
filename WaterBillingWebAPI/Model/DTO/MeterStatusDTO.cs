using WaterBillingApp.Data.Entities;

namespace WaterBillingWebAPI.Model.DTO
{
    public class MeterStatusDTO
    {
        public int Id { get; set; }
        public string InstallationAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public MeterStatus Status { get; set; }
    }
}
