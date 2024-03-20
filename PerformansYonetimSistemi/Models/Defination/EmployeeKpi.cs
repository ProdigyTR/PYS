using System;
using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.Defination
{
    public class EmployeeKpi:Base
    {
        [Key]
        public int Id { get; set; }
        public string TC { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
