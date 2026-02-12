using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabIssueSystem.DAL;
using LabIssueSystem.Models;
using LabIssueSystem.Models.ViewModels;

namespace LabIssueSystem.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly LabIssueContext _context;

        public FacultyController(LabIssueContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();

            // Calculate statistics
            var resolvedTickets = tickets.Where(t => t.Status == "Resolved" && t.ResolvedDate.HasValue).ToList();
            double avgResolutionTime = 0;
            if (resolvedTickets.Any())
            {
                avgResolutionTime = resolvedTickets.Average(t => (t.ResolvedDate!.Value - t.ReportedDate).TotalHours);
            }

            // Network team performance
            var networkTeamMembers = users.Where(u => u.Role == "NetworkTeam").ToList();
            var teamPerformance = networkTeamMembers.Select(member => new
            {
                Name = member.FullName ?? member.Username,
                ResolvedCount = tickets.Count(t => t.AssignedTo == member.UserId && t.Status == "Resolved"),
                InProgressCount = tickets.Count(t => t.AssignedTo == member.UserId && t.Status == "InProgress")
            }).ToList();

            var model = new DashboardViewModel
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == "Open"),
                InProgressTickets = tickets.Count(t => t.Status == "InProgress"),
                ResolvedTickets = tickets.Count(t => t.Status == "Resolved"),
                AverageResolutionTime = Math.Round(avgResolutionTime, 2),
                HighPriorityTickets = tickets.Count(t => t.Priority == "High" && t.Status != "Resolved"),
                MediumPriorityTickets = tickets.Count(t => t.Priority == "Medium" && t.Status != "Resolved"),
                LowPriorityTickets = tickets.Count(t => t.Priority == "Low" && t.Status != "Resolved"),
                RecentTickets = tickets.OrderByDescending(t => t.ReportedDate)
                    .Take(5)
                    .Select(t => new TicketViewModel
                    {
                        TicketId = t.TicketId,
                        IPAddress = t.IPAddress,
                        IssueTitle = t.IssueTitle,
                        Status = t.Status,
                        ReportedDate = t.ReportedDate,
                        Priority = t.Priority,
                        ReportedByName = t.Reporter?.FullName ?? t.Reporter?.Username,
                        AssignedToName = t.Assignee?.FullName ?? t.Assignee?.Username
                    }).ToList()
            };

            ViewBag.TeamPerformance = teamPerformance;
            ViewBag.TotalStudents = users.Count(u => u.Role == "Student");
            ViewBag.TotalNetworkTeam = networkTeamMembers.Count;

            return View(model);
        }

        public async Task<IActionResult> MonitorIssues(string? searchString, string? statusFilter, string? priorityFilter, string? sortBy)
        {
            var tickets = _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t =>
                    t.TicketId.ToString().Contains(searchString) ||
                    t.IPAddress.Contains(searchString) ||
                    t.IssueTitle.Contains(searchString) ||
                    (t.Reporter != null && (t.Reporter.FullName != null && t.Reporter.FullName.Contains(searchString))) ||
                    (t.Reporter != null && t.Reporter.Username.Contains(searchString)));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                tickets = tickets.Where(t => t.Status == statusFilter);
            }

            // Apply priority filter
            if (!string.IsNullOrEmpty(priorityFilter) && priorityFilter != "All")
            {
                tickets = tickets.Where(t => t.Priority == priorityFilter);
            }

            // Apply sorting
            tickets = sortBy switch
            {
                "Oldest" => tickets.OrderBy(t => t.ReportedDate),
                "Priority" => tickets.OrderByDescending(t => t.Priority == "High" ? 1 : t.Priority == "Medium" ? 2 : 3),
                "Resolved" => tickets.OrderByDescending(t => t.ResolvedDate),
                _ => tickets.OrderByDescending(t => t.ReportedDate)
            };

            var ticketList = await tickets.ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.PriorityFilter = priorityFilter;
            ViewBag.SearchString = searchString;
            ViewBag.SortBy = sortBy;

            return View(ticketList);
        }

        public async Task<IActionResult> TicketDetails(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                IPAddress = ticket.IPAddress,
                IssueTitle = ticket.IssueTitle,
                IssueDescription = ticket.IssueDescription,
                Status = ticket.Status,
                Priority = ticket.Priority,
                ReportedDate = ticket.ReportedDate,
                ResolvedDate = ticket.ResolvedDate,
                ReportedByName = ticket.Reporter?.FullName ?? ticket.Reporter?.Username,
                AssignedToName = ticket.Assignee?.FullName ?? ticket.Assignee?.Username,
                ResolutionNotes = ticket.ResolutionNotes,
                LabName = ticket.LabName,
                ComputerName = ticket.ComputerName
            };

            return View(model);
        }

        public async Task<IActionResult> Reports()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .ToListAsync();

            // Group by lab
            var ticketsByLab = tickets.GroupBy(t => t.LabName ?? "Unknown")
                .Select(g => new { LabName = g.Key, Count = g.Count() })
                .ToList();

            // Group by status
            var ticketsByStatus = tickets.GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Group by priority
            var ticketsByPriority = tickets.GroupBy(t => t.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToList();

            // Resolution time analysis
            var resolvedTickets = tickets.Where(t => t.Status == "Resolved" && t.ResolvedDate.HasValue).ToList();
            var resolutionTimeAnalysis = resolvedTickets.Select(t => new
            {
                TicketId = t.TicketId,
                IssueTitle = t.IssueTitle,
                LabName = t.LabName,
                ReportedDate = t.ReportedDate,
                ResolvedDate = t.ResolvedDate,
                ResolutionTimeHours = Math.Round((t.ResolvedDate!.Value - t.ReportedDate).TotalHours, 2)
            }).OrderByDescending(r => r.ResolutionTimeHours).ToList();

            ViewBag.TicketsByLab = ticketsByLab;
            ViewBag.TicketsByStatus = ticketsByStatus;
            ViewBag.TicketsByPriority = ticketsByPriority;
            ViewBag.ResolutionTimeAnalysis = resolutionTimeAnalysis;
            ViewBag.AverageResolutionTime = resolvedTickets.Any() 
                ? Math.Round(resolvedTickets.Average(t => (t.ResolvedDate!.Value - t.ReportedDate).TotalHours), 2) 
                : 0;

            return View();
        }
    }
}
