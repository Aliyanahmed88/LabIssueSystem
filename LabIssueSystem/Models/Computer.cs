using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LabIssueSystem.Models
{
    public class Computer
    {
        [Key]
        public int ComputerId { get; set; }

        [Required]
        [StringLength(15)]
        public string IPAddress { get; set; }

        [StringLength(50)]
        public string? ComputerName { get; set; }

        [StringLength(50)]
        public string? LabName { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
