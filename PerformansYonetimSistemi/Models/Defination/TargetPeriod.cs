using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class TargetPeriod:Base
    {
        [Key]
        public int Id { get; set; }
        public string Department { get; set; } = "";
        public string Employee { get; set; } = "";
        public DateTime StartDate { get; set; }= DateTime.Now;
        public DateTime EndDate { get; set; }=DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
