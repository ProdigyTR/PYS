using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Department:Base
    {
        [Key]
        [Required]
        [DisplayName("Kod")]
        public string Code { get; set; }
        [Required]
        [DisplayName("Departman Adı")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Aktif")]
        public bool IsActive { get; set; }=true;
    }
}