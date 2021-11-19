using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibshelfAPI.Models;

public sealed class Shelf
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    [NotMapped] public int TotalBook => Books.Count;

    [JsonIgnore] public List<Book> Books { get; set; } = null!;
}