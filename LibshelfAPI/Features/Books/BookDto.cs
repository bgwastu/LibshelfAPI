using System.ComponentModel.DataAnnotations;
using LibshelfAPI.Models;

namespace LibshelfAPI.Features.Books;

public record BookRequest([Required] string Title, string? Isbn, string? CoverUrl, string? Description,
    List<string>? Genres,
    BookStatus Status, List<string>? Authors, int PageCount, List<Guid>? ShelfIds, DateTime? DateReadUtc, DateTime? DateFinishedUtc
);

public class BookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public List<string>? Genres { get; set; }
    public BookStatus? Status { get; set; }
    public List<string>? Authors { get; set; }
    public int PageCount { get; set; }

    public DateTime? DateReadUtc { get; set; }
    public DateTime? DateFinishedUtc { get; set; }

    public static BookResponse FromBook(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            CoverUrl = book.CoverUrl,
            Description = book.Description,
            Genres = book.Genres,
            Status = book.Status,
            Authors = book.Authors,
            PageCount = book.PageCount,
            DateReadUtc = book.DateReadUtc,
            DateFinishedUtc = book.DateFinishedUtc
        };
    }
}