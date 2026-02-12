using System.ComponentModel.DataAnnotations;

namespace LabIssueSystem.Models.ViewModels
{
    public class ReportIssueViewModel
    {
        [Required(ErrorMessage = "IP Address is required")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Invalid IP Address format")]
        public string IPAddress { get; set; }

        [Required(ErrorMessage = "Issue Title is required")]
        [StringLength(200)]
        public string IssueTitle { get; set; }

        [Required(ErrorMessage = "Issue Description is required")]
        [StringLength(1000)]
        public string IssueDescription { get; set; }

        [Required(ErrorMessage = "Lab Name is required")]
        public string LabName { get; set; }

        [StringLength(50)]
        public string ComputerName { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";
    }
}
