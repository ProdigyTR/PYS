using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Target:Base
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Explanation { get; set; }
        public required string Employee { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal CompletedRatio { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}