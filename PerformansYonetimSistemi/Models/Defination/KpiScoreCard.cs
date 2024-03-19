using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class KpiScoreCard:Base
    {
        [Key]
        public int Id { get; set; }
        public int KpiId { get; set; }
        public int Target { get; set; }
        public int Progress { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int Ratio { get; set; } = 0;
    }
}
