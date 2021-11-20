using LibshelfAPI.Models;

namespace LibshelfAPI.Features.Books;

public record UserRegister(string Name, string Email, string Password);

public record UserLogin(string Email, string Password);

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Key { get; set; } = null!;

    public static UserResponse FromAuth(User user, string key)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Key = key
        };
    }
}