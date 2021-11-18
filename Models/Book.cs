using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions;

namespace LibshelfAPI.Models;

public enum BookStatus
{
    WantToRead,
    Reading,
    Read
}

public class Book : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public string CoverUrl { get; set; }
    public string Description { get; set; }
    public List<string> Genres { get; set; }
    public BookStatus Status { get; set; }
    public List<string> Authors { get; set; }
    public int PageCount { get; set; }
    
    public List<Shelf> Shelves => new();
}