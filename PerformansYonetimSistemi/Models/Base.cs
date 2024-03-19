namespace PerformansYonetimSistemi.Models
{
    public class Base
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "mert.bagbasi";
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}

