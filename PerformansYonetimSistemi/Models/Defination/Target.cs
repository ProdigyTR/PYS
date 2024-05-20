using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Target:Base
    {
        [Key]
        public int Id { get; set; }
        public int TargetPeriodId { get;set; }
        public required string KpiCode { get; set; }
        public required string Explanation { get; set; }
        public required string Employee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}