using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LibshelfAPI.Features.Books;

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
    [Required] public string Title { get; set; }
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public List<string>? Genres { get; set; }
    [Required] public BookStatus Status { get; set; }
    public List<string>? Authors { get; set; }
    public int PageCount { get; set; }
    public virtual List<Shelf> Shelves { get; set; } = null!;


    public static Book From(BookRequest bookRequest)
    {
        return new Book
        {
            Title = bookRequest.Title,
            Isbn = bookRequest.Isbn,
            CoverUrl = bookRequest.CoverUrl,
            Description = bookRequest.Description,
            Genres = bookRequest.Genres,
            Status = (BookStatus) Enum.Parse(typeof(BookStatus), bookRequest.Status),
            Authors = bookRequest.Authors,
            PageCount = bookRequest.PageCount,
        };
    }
}