using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VacationManager.Models;

namespace VacationManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<RequestLog> RequestLogs { get; set; } = null!; // ✅ НОВО

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ⬚ Връзки между таблици
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Teams)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamLead)
                .WithMany()
                .HasForeignKey(t => t.TeamLeadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany()
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Teams)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⬚ По подразбиране дата на създаване
            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // ✅ История на заявките (RequestLog)
            modelBuilder.Entity<RequestLog>()
                .HasOne(log => log.LeaveRequest)
                .WithMany()
                .HasForeignKey(log => log.LeaveRequestId);

            modelBuilder.Entity<RequestLog>()
                .HasOne(log => log.PerformedBy)
                .WithMany()
                .HasForeignKey(log => log.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ⬚ Seed роли
            SeedRoles(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "admin-role-id", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "teamlead-role-id", Name = "TeamLead", NormalizedName = "TEAMLEAD" },
                new IdentityRole { Id = "developer-role-id", Name = "Developer", NormalizedName = "DEVELOPER" }
            );
        }
    }
}
