using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class KPI:Base
    {
        [Key]
        [Required]
        [DisplayName("Kod")]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Departmant { get; set; }

        [Required]
        [DisplayName("Aktif")]
        public bool IsActive { get; set; } = true;
    }
}
