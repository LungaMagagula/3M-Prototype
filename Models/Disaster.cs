using System.ComponentModel.DataAnnotations;

namespace DesasterAlleviationFund.Models
{
    public class Disaster
    {
        [Key]
        public int DisasterId { get; set; }
        public string Email { get; set;}
        public DateTime? Start_date { get; set;}
        public DateTime? End_date { get; set;}
        public string Location { get; set;}
        public string Description { get; set;}
        public string Aid_types { get; set;}

        
    }
}
