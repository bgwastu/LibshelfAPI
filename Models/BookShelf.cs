namespace LibshelfAPI.Models;

public class BookShelf
{
    public Guid BookId { get; set; }
    public Guid ShelfId { get; set; }

    public List<Shelf> Shelves { get; set; }
    public List<Book> Books { get; set; }
    
}