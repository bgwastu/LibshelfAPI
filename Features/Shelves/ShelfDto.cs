using System.ComponentModel.DataAnnotations;
using LibshelfAPI.Features.Books;
using LibshelfAPI.Models;

namespace LibshelfAPI.Features.Shelves;

public record ShelfRequest([Required] string Name);
public record ShelfBookRequest([Required] Guid BookId);

public class ShelfResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalBook => _books.Count;
    private List<BookResponse> _books = null!;

    public static ShelfResponse FromShelf(Shelf shelf)
    {
        return new ShelfResponse
        {
            Id = shelf.Id,
            Name = shelf.Name,
            _books = shelf.Books.Select(BookResponse.FromBook).ToList()
        };
    }
}