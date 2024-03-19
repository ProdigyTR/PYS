using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Login
{
    public class IK_User:Base
    {
        [Key]
        public string userName { get; set; }
        public string password { get; set; }
        public string? eMail { get; set; }
        public string? phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? department { get; set; }
        public string? title { get; set; }
        public Int16 admin { get; set; }
        public Int16 active { get; set; }
    }
}
