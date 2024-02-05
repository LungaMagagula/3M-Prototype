using System.ComponentModel.DataAnnotations;

namespace DesasterAlleviationFund.Models
{
    public class Monetory
    {
        [Key]
        public int ID { get; set; }
        public string Email { get; set; }
        public string DonorName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string dropdown { get; set; }
        public string DonorNameInput { get; set; }
    }
}
