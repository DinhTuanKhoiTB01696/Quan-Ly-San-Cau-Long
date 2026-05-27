using BadmintonApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Court)
            .WithMany(c => c.Matches)
            .HasForeignKey(m => m.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.HostUser)
            .WithMany()
            .HasForeignKey(m => m.HostUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Match)
            .WithMany(m => m.Reports)
            .HasForeignKey(r => r.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportedByUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Court>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Match>()
            .Property(m => m.Cost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MatchParticipant>()
            .HasKey(mp => new { mp.MatchId, mp.UserId });

        modelBuilder.Entity<MatchParticipant>()
            .HasOne(mp => mp.Match)
            .WithMany(m => m.Participants)
            .HasForeignKey(mp => mp.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchParticipant>()
            .HasOne(mp => mp.User)
            .WithMany(u => u.JoinedMatches)
            .HasForeignKey(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
