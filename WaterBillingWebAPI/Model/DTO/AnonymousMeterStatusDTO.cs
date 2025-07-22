using WaterBillingApp.Data.Entities;

namespace WaterBillingWebAPI.Model.DTO
{
    public class AnonymousMeterStatusDTO
    {
        public int MeterId { get; set; }
        public string Address { get; set; }
        public DateTime RequestDate { get; set; }
        public MeterStatus Status { get; set; }
    }
}
