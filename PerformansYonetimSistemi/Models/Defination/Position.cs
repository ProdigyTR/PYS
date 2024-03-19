using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Position:Base
    {
        [Key]
        [Required]
        [DisplayName("Kod")]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Aktif")]
        public bool IsActive { get; set; } = true;
        [Required]
        public string DepartmentCode { get; set; } 

    }
}
