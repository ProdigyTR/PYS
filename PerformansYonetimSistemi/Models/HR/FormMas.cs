using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.HR
{
    public class FormMas:Base
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Explanation { get; set; }
    }
}
