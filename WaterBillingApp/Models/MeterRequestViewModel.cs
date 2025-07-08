using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
   
    public class MeterRequestViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NIF { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }
}
