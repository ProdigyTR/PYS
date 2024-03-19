using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class Employee : Base
    {
        [Key]
        public required string TC { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Position { get; set; }
        public required string Department { get; set; }
        public DateTime DateOfRecruitment { get; set; }
        public required string Mail { get; set; }
        public int TotalWorkExperience { get; set; } = 0;
        public string Responsible { get; set; } = "";
        public bool IsManager { get; set; } = false;
        public bool IsGM { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
