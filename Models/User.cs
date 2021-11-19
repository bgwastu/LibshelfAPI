using LibshelfAPI.Features.Books;

namespace LibshelfAPI.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
    
    public static User FromUserRegister(UserRegister userRegister)
    {
        var (name, email, password) = userRegister;
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password),
            CreatedAt = DateTime.UtcNow
        };
    }
}