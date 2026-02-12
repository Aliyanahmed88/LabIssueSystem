using System.ComponentModel.DataAnnotations;

namespace LabIssueSystem.Models.ViewModels
{
    public class UpdateStatusViewModel
    {
        public int TicketId { get; set; }
        public string IPAddress { get; set; }
        public string IssueTitle { get; set; }
        public string IssueDescription { get; set; }
        public string CurrentStatus { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string NewStatus { get; set; }

        public string ResolutionNotes { get; set; }
    }
}
