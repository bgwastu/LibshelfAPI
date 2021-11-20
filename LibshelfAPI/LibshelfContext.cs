using LibshelfAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LibshelfAPI;

public class LibshelfContext : DbContext
{
    public LibshelfContext(DbContextOptions<LibshelfContext> options) : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; } = null!;
    public virtual DbSet<Shelf> Shelves { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var splitStringConverter =
            new ValueConverter<List<string>, string>(v => string.Join(";", v), v => v.Split(new[] {';'}).ToList());
        modelBuilder.Entity<Book>().Property(nameof(Book.Authors))
            .HasColumnType("varchar")
            .HasConversion(splitStringConverter);
        modelBuilder.Entity<Book>().Property(nameof(Book.Genres))
            .HasColumnType("varchar")
            .HasConversion(splitStringConverter);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibshelfContext).Assembly);
        modelBuilder
            .Entity<Book>()
            .Property(b => b.Status)
            .HasConversion(
                bs => bs.ToString(),
                s => (BookStatus) Enum.Parse(typeof(BookStatus), s)
            );
    }
}