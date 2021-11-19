using LibshelfAPI.Features.Books;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Features.Shelves;

[Route("/api/shelves")]
[ApiController]
public class ShelvesController : ControllerBase
{
    private readonly LibshelfContext _context;

    public ShelvesController(LibshelfContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetShelves()
    {
        var shelves = await _context.Shelves.Include(s => s.Books).ToListAsync();
        return Ok(shelves.Select(ShelfResponse.FromShelf));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var shelf = await _context.Shelves.Include(s => s.Books).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        return Ok(shelf);
    }

    [HttpGet]
    [Route("{id:Guid}/books")]
    public async Task<IActionResult> GetBooks([FromRoute] Guid id)
    {
        var shelf = await _context.Shelves.Include(s => s.Books).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        return Ok(shelf.Books.Select(BookResponse.FromBook));
    }

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
        };

        _context.Shelves.Add(shelf);
        await _context.SaveChangesAsync();

        // Get shelf with books
        shelf = await _context.Shelves.Include(s => s.Books).FirstOrDefaultAsync(s => s.Id == shelf.Id);

        return CreatedAtAction("Get", new {id = shelf!.Id}, ShelfResponse.FromShelf(shelf));
    }

    /// <summary>
    /// Add a book to a shelf
    /// </summary>
    [HttpPost("{id:Guid}/books")]
    public async Task<IActionResult> CreateBooks([FromRoute] Guid id, [FromBody] ShelfBookRequest shelfBookRequest)
    {
        // Check shelf exists
        var shelf = await _context.Shelves.Include(s => s.Books).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        // Check book exists
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == shelfBookRequest.BookId);
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
    /// Delete book from shelf
    /// </summary>
    [HttpDelete("{id:Guid}/books/{bookId:Guid}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id, [FromRoute] Guid bookId)
    {
        // Check shelf exists
        var shelf = await _context.Shelves.Include(s => s.Books).FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        // Check book exists
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
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


    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ShelfRequest shelfRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shelf = await _context.Shelves.FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        shelf.Name = shelfRequest.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var shelf = await _context.Shelves.FirstOrDefaultAsync(s => s.Id == id);
        if (shelf == null)
        {
            return NotFound();
        }

        _context.Shelves.Remove(shelf);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}