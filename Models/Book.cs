using System.ComponentModel.DataAnnotations;
using LibshelfAPI.Features.Books;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibshelfAPI.Models;

[JsonConverter(typeof(StringEnumConverter), true)]
public enum BookStatus
{
    WantToRead,
    Reading,
    Read
}

public sealed class Book
{
    public Guid Id { get; set; }
    [Required] public string Title { get; set; } = null!;
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public List<string>? Genres { get; set; }

    [Required] public BookStatus Status { get; set; }
    public List<string>? Authors { get; set; }
    public int PageCount { get; set; }

    public DateTime? DateReadUtc { get; set; }
    public DateTime? DateFinishedUtc { get; set; }
    public List<Shelf> Shelves { get; set; } = null!;

    public static Book FromBookRequest(BookRequest bookRequest)
    {
        return new Book
        {
            Title = bookRequest.Title,
            Isbn = bookRequest.Isbn,
            CoverUrl = bookRequest.CoverUrl,
            Description = bookRequest.Description,
            Genres = bookRequest.Genres,
            Status = bookRequest.Status,
            Authors = bookRequest.Authors,
            PageCount = bookRequest.PageCount,
            DateReadUtc = bookRequest.DateReadUtc,
            DateFinishedUtc = bookRequest.DateFinishedUtc,
        };
    }

    public void ReplaceWithBookRequest(BookRequest bookRequest)
    {
        Title = bookRequest.Title;
        Isbn = bookRequest.Isbn;
        CoverUrl = bookRequest.CoverUrl;
        Description = bookRequest.Description;
        Genres = bookRequest.Genres;
        Status = bookRequest.Status;
        Authors = bookRequest.Authors;
        PageCount = bookRequest.PageCount;
        DateReadUtc = bookRequest.DateReadUtc;
        DateFinishedUtc = bookRequest.DateFinishedUtc;
    }
}