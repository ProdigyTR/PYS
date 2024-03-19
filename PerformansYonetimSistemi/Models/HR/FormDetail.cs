using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.HR
{
    public class FormDetail:Base
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MasId { get; set; }

        public string Explanation { get; set; }

        [Required]
        public int TextLevel { get; set; }

        public string? Title { get; set; }

        public string? LowerTitle { get; set; }

        [Required]
        public int Sequence { get; set; }
    }
}
