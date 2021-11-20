using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using LibshelfAPI.Features.Books;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibshelfAPI.Features.Users;

[Route("/api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly LibshelfContext _context;
    private readonly IConfiguration _configuration;

    public UsersController(LibshelfContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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
        return Ok(UserResponse.FromAuth(user, GenerateToken(user)));
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
        
        // Check email
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

        return Ok(UserResponse.FromAuth(user, GenerateToken(user)));
    }

    private string GenerateToken(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}