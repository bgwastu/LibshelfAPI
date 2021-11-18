using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibshelfAPI.Models;

public class Shelf : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [NotMapped] public int TotalBook => Books.Count;

    [JsonIgnore] public virtual List<Book> Books { get; set; } = null!;
}