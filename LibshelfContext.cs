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
}