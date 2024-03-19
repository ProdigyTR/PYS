namespace PerformansYonetimSistemi.Models.HR
{
    public class NeedToFillDepartmentManager:Base
    {
        public int Id { get; set; }
        public int MasId { get; set; }
        public string Guid { get; set; }
        public string ustGorev { get; set; }
        public string? ustGorevEvet { get; set; }
        public string? ustGorevHayir { get; set; }
        public string ekGorev { get; set; }
        public string? ekGorevEvet { get; set; }
        public string? ekGorevHayir { get; set; }
        public string? egitimOneri1 { get; set; }
        public string? egitimOneri2 { get; set; }
        public string? egitimOneri3 { get; set; }
        public string? egitimOneri4 { get; set; }
        public string? egitimOneri5 { get; set; }
        public string? alinacakAksiyon1 { get; set; }
        public string? alinacakAksiyon2 { get; set; }
        public string? alinacakAksiyon3 { get; set; }
        public string? alinacakAksiyon4 { get; set; }
        public string? alinacakAksiyon5 { get; set; }
    }
}
