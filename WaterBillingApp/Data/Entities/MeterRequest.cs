using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    public enum MeterRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public class MeterRequest
    {
        public int Id { get; set; }

        [Required]
        public string RequesterName { get; set; }

        [Required]
        [EmailAddress]
        public string RequesterEmail { get; set; }

        [Required]
        public string NIF { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public DateTime RequestDate { get; set; }

        public MeterRequestStatus Status { get; set; }
    }
}
