using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabIssueSystem.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [StringLength(15)]
        public string IPAddress { get; set; }

        [Required]
        [StringLength(200)]
        public string IssueTitle { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open"; // Open, InProgress, Resolved

        [StringLength(20)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High

        public DateTime ReportedDate { get; set; } = DateTime.Now;

        public DateTime? ResolvedDate { get; set; }

        [Required]
        public int ReportedBy { get; set; }

        public int? AssignedTo { get; set; }

        public string? ResolutionNotes { get; set; }

        [StringLength(50)]
        public string? LabName { get; set; }

        [StringLength(50)]
        public string? ComputerName { get; set; }

        // Navigation properties
        [ForeignKey("ReportedBy")]
        public virtual User? Reporter { get; set; }

        [ForeignKey("AssignedTo")]
        public virtual User? Assignee { get; set; }

    }
}
