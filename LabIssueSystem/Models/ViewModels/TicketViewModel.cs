using System;

namespace LabIssueSystem.Models.ViewModels
{
    public class TicketViewModel
    {
        public int TicketId { get; set; }
        public string IPAddress { get; set; }
        public string IssueTitle { get; set; }
        public string IssueDescription { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string ReportedByName { get; set; }
        public string AssignedToName { get; set; }
        public string ResolutionNotes { get; set; }
        public string LabName { get; set; }
        public string ComputerName { get; set; }
    }
}
