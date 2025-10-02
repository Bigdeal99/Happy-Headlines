using DraftService.Models;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Data;

public class DraftDbContext : DbContext
{
    public DraftDbContext(DbContextOptions<DraftDbContext> options) : base(options) { }

    public DbSet<Draft> Drafts => Set<Draft>();

    public override int SaveChanges()
    {
        TouchTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        TouchTimestamps();
        return await base.SaveChangesAsync(token);
    }

    private void TouchTimestamps()
    {
        var entries = ChangeTracker.Entries<Draft>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var e in entries)
            e.Entity.UpdatedAt = DateTime.UtcNow;
    }
}
