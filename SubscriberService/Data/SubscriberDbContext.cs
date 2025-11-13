using Microsoft.EntityFrameworkCore;
using SubscriberService.Models;

namespace SubscriberService.Data;

public class SubscriberDbContext : DbContext
{
    public SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : base(options) { }

    public DbSet<Subscriber> Subscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });
    }
}

