using LibshelfAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI;

public class LibshelfContext : DbContext
{
    public LibshelfContext(DbContextOptions<LibshelfContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Shelf> Shelves { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Book>()
            .Property(b => b.Status)
            .HasConversion(
                bs => bs.ToString(),
                s => (BookStatus) Enum.Parse(typeof(BookStatus), s)
            );
    }


    public override int SaveChanges()
    {
        var newEntities = this.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Added &&
                     x.Entity is BaseEntity
            )
            .Select(x => x.Entity as BaseEntity);

        var modifiedEntities = this.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Modified &&
                     x.Entity is BaseEntity
            )
            .Select(x => x.Entity as BaseEntity);

        foreach (var newEntity in newEntities)
        {
            if (newEntity == null) continue;
            newEntity.CreatedDate = DateTime.UtcNow;
            newEntity.UpdatedDate = DateTime.UtcNow;
        }

        foreach (var modifiedEntity in modifiedEntities)
        {
            if (modifiedEntity != null) modifiedEntity.UpdatedDate = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }
}