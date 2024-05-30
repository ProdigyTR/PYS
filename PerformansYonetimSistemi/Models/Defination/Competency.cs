using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Competency:Base
    {
        [Key]
        public int Id { get; set; }
        public int TargetPeriodId { get; set; }
        public int MasId { get; set; }
        public int DetailId { get; set; }
        public string? Explanation { get; set; }
        public int TextLevel { get; set; }
        public string? Title { get; set; }
        public string? LowerTitle { get; set; }
        public int Sequence { get; set; }
        public string GivenPoint { get; set; } = "0";
        public string Employee { get; set; } = "";
    }
}
