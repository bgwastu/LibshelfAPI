using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibshelfAPI.Models;

public sealed class Shelf
{
    [Key] public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
    [JsonIgnore] public List<Book> Books { get; set; } = null!;
}