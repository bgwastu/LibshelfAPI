namespace LibshelfAPI.Models;

public class BookShelf
{
    public Guid BookId { get; set; }
    public Guid ShelfId { get; set; }

    public virtual List<Shelf> Shelves { get; set; } = null!;
    public virtual List<Book> Books { get; set; } = null!;
}