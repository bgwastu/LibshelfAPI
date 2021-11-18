using System.ComponentModel.DataAnnotations.Schema;

namespace LibshelfAPI.Models;

public class Shelf : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [NotMapped] public int TotalBook => Books.Count;

    public List<Book> Books { get; set; }
}