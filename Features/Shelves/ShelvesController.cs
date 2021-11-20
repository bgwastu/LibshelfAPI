using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LibshelfAPI.Features.Books;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Features.Shelves;

[Route("/api/shelves")]
[ApiController]
[Authorize]
public class ShelvesController : ControllerBase
{
    private readonly LibshelfContext _context;

    public ShelvesController(LibshelfContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all shelves.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelves = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books).ToListAsync();
        return Ok(shelves.Select(ShelfResponse.FromShelf));
    }

    /// <summary>
    /// Get a shelf by id.
    /// </summary>
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        return Ok(ShelfResponse.FromShelf(shelf));
    }

    /// <summary>
    /// Get list of books in a shelf.
    /// </summary>
    [HttpGet]
    [Route("{id:Guid}/books")]
    public async Task<IActionResult> GetBooks([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        return Ok(shelf.Books.Select(BookResponse.FromBook));
    }

    /// <summary>
    /// Create a new shelf.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShelfRequest shelfRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shelf = new Shelf
        {
            Name = shelfRequest.Name,
            UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!)
        };

        _context.Shelves.Add(shelf);
        await _context.SaveChangesAsync();

        // Get shelf with books
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        shelf = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books)
            .FirstOrDefaultAsync(s => s.Id == shelf.Id);

        return CreatedAtAction("Get", new {id = shelf!.Id}, ShelfResponse.FromShelf(shelf));
    }

    /// <summary>
    /// Add a book to a shelf.
    /// </summary>
    [HttpPost("{id:Guid}/books")]
    public async Task<IActionResult> CreateBooks([FromRoute] Guid id, [FromQuery] [Required] Guid bookId)
    {
        // Check shelf exists
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        // Check book exists

        var book = await _context.Books.Where(s => s.UserId == userId).FirstOrDefaultAsync(b => b.Id == bookId);
        if (book == null)
        {
            return NotFound();
        }

        // Check book already exist in shelf
        if (shelf.Books.Any(b => b.Id == book.Id))
        {
            return BadRequest("Book already exists in shelf");
        }

        // Add book to shelf
        shelf.Books.Add(book);

        _context.Update(shelf);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete book from shelf.
    /// </summary>
    [HttpDelete("{id:Guid}/books/{bookId:Guid}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id, [FromRoute] Guid bookId)
    {
        // Check shelf exists
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).Include(s => s.Books)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        // Check book exists
        var book = await _context.Books.Where(s => s.UserId == userId).FirstOrDefaultAsync(b => b.Id == bookId);
        if (book == null)
        {
            return NotFound();
        }

        // Remove book from shelf and save the change
        shelf.Books.Remove(book);
        _context.Update(shelf);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    /// <summary>
    /// Update shelf.
    /// </summary>
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ShelfRequest shelfRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        shelf.Name = shelfRequest.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete shelf.
    /// </summary>
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var shelf = await _context.Shelves.Where(s => s.UserId == userId).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        _context.Shelves.Remove(shelf);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}