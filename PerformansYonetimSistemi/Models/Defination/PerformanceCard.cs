using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class PerformanceCard:Base
    {
        [Key]
        public int Id { get; set; }
        public int KpiId { get; set; }
        public int TargetId { get; set; }
        public int Point { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int Ratio { get; set; } = 0;
    }
}
