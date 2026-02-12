using Microsoft.EntityFrameworkCore;
using LabIssueSystem.Models;

namespace LabIssueSystem.DAL
{
    public class LabIssueContext : DbContext
    {
        public LabIssueContext(DbContextOptions<LabIssueContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Computer> Computers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Ticket entity
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(t => t.Reporter)
                      .WithMany(u => u.ReportedTickets)
                      .HasForeignKey(t => t.ReportedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Assignee)
                      .WithMany(u => u.AssignedTickets)
                      .HasForeignKey(t => t.AssignedTo)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Computer entity
            modelBuilder.Entity<Computer>(entity =>
            {
                entity.HasIndex(e => e.IPAddress).IsUnique();
            });
        }
    }
}
