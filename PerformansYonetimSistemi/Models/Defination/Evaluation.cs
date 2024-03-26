using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Evaluation:Base
    {
        [Key]
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Explanation { get; set; }
    }
}
