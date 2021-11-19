using System.Text.RegularExpressions;
using LibshelfAPI.Features.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Features.Users;

[Route("/api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly LibshelfContext _context;

    public UsersController(LibshelfContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
    {
        // Check if email is valid using regex
        if (!Regex.IsMatch(userRegister.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
        {
            return BadRequest("Email is not valid");
        }

        // Check if email is already in use
        if (await _context.Users.AnyAsync(u => u.Email == userRegister.Email))
        {
            return BadRequest("Email is already in use");
        }

        // Check if password is long enough
        if (userRegister.Password.Length < 8)
        {
            return BadRequest("Password is too short");
        }

        var user = Models.User.FromUserRegister(userRegister);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        var (email, password) = userLogin;
        // Check if email is valid using regex
        if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
        {
            return BadRequest("Email is not valid");
        }

        // Check if password is long enough
        if (password.Length < 8)
        {
            return BadRequest("Password is too short");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return BadRequest("Email is incorrect");
        }
        
        // Check password based on bcrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return BadRequest("Password is incorrect");
        }

        return Ok(UserResponse.FromLogin(user, "keyplaceholder"));
    }
}