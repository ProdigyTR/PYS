using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Login
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        [Required]
        public string EMail { get; set; } = "";
        public int UserGroupId { get; set; } = 0;
        public string Image { get; set; } = "/Upload/avatar.PNG";
        public bool Active { get; set; } = true;
        public bool Executive { get; set; } = false;
        public bool Manager { get; set; } = false;
        public DateTime CreateTime { get; set; }= DateTime.Now;
    }
}
