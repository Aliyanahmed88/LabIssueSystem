using System;
using System.Collections.Generic;

namespace LabIssueSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public double AverageResolutionTime { get; set; }
        public List<TicketViewModel> RecentTickets { get; set; }
        public List<TicketViewModel> AllTickets { get; set; }
        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }
    }
}
