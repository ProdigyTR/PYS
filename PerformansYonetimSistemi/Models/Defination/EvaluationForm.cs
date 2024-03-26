using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class EvaluationForm : Base
    {
        [Key]
        public int Id { get; set; }
        public string EvaluationCode { get; set; }
        public int FormId { get; set; }
    }
}
