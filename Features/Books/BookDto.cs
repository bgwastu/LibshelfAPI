using System.ComponentModel.DataAnnotations;
using LibshelfAPI.Models;

namespace LibshelfAPI.Features.Books;

public record BookRequest([Required] string Title, string? Isbn, string? CoverUrl, string? Description,
    List<string>? Genres,
    string Status, List<string>? Authors, int PageCount, List<Guid>? ShelfIds
);

public class BookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Isbn { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public List<string>? Genres { get; set; }
    public String Status { get; set; }
    public List<string>? Authors { get; set; }
    public int PageCount { get; set; }

    public static BookResponse From(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            CoverUrl = book.CoverUrl,
            Description = book.Description,
            Genres = book.Genres,
            Status = book.Status.ToString(),
            Authors = book.Authors,
            PageCount = book.PageCount,
        };
    }
}