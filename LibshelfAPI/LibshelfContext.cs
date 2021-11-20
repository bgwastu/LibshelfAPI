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
    public DbSet<User> Users { get; set; } = null!;

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
}