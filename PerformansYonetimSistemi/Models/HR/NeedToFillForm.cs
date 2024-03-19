﻿using System.ComponentModel.DataAnnotations;

namespace PerformansYonetimSistemi.Models.HR
{
    public class NeedToFillForm:Base
    {
        [Key]
        public int Id { get; set; }
        public int MasId { get; set; }
        public int DetailId { get; set; }
        public string Guid { get; set; }
        public string? Explanation { get; set; }
        public int TextLevel { get; set; }
        public string? Title { get; set; }
        public string? LowerTitle { get; set; }
        public int Sequence { get; set; }
        public string? GivenPoint { get; set; }
        public string Employee { get; set; }
        public int? Manager { get; set; }
        public DateTime? FormDate { get; set; }
    }
}
