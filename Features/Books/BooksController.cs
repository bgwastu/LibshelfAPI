using System.Linq;
using System.Security.Claims;
using ImageMagick;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Features.Books;

[Route("/api/books")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly LibshelfContext _context;

    public BooksController(LibshelfContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all books, search by title, author, or ISBN. Returns list of BookResponse.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? query)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var books = await _context.Books.Where(b => b.UserId == userId).ToListAsync();
        if (query != null)
        {
            books = books.Where(b =>
                b.Authors != null && b.Isbn != null &&
                (b.Title.Contains(query) || b.Authors.Contains(query) || b.Isbn.Contains(query))).ToList();
        }

        return Ok(books.Select(b => BookResponse.FromBook(b)));
    }

    /// <summary>
    /// Get book by id.
    /// </summary>
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        var bookResponse = BookResponse.FromBook(book);

        return Ok(bookResponse);
    }

    /// <summary>
    /// Get book shelves by id.
    /// </summary>
    [HttpGet("{id:Guid}/shelves")]
    public async Task<IActionResult> GetShelves(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = await _context.Books.Where(b => b.UserId == userId).Include(b => b.Shelves)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return Ok(book.Shelves);
    }

    /// <summary>
    /// Upload cover image by book id and convert into .webp.
    /// </summary>
    [HttpPost("{id:Guid}/cover")]
    public async Task<IActionResult> UploadCover([FromForm] IFormFile file, Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        var fileName = $"{id}.webp";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        using (var image = new MagickImage(filePath))
        {
            image.Format = MagickFormat.WebP;
            await image.WriteAsync(filePath);
        }

        return Ok();
    }

    /// <summary>
    /// Delete cover image by book id.
    /// </summary>
    [HttpDelete("{id:Guid}/cover")]
    public async Task<IActionResult> DeleteCover(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        // Check if cover url already exists
        if (string.IsNullOrEmpty(book.CoverUrl))
        {
            return BadRequest("Cover url is empty");
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images",
            book.CoverUrl);
        System.IO.File.Delete(filePath);

        book.CoverUrl = null;
        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Replace and upload new cover image by book id.
    /// </summary>
    [HttpPatch("{id:Guid}/cover")]
    public async Task<IActionResult> ReplaceCover(Guid id, [FromForm] IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        var fileName = $"{id}.webp";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        using (var image = new MagickImage(filePath))
        {
            image.Format = MagickFormat.WebP;
            await image.WriteAsync(filePath);
        }

        book.CoverUrl = fileName;
        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Create new book.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] BookRequest bookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var book = Book.Map(bookRequest, userId);

        if (bookRequest.ShelfIds is not null)
        {
            // Validate shelfIds
            var shelves = await _context.Shelves.Where(s => bookRequest.ShelfIds.Contains(s.Id)).ToListAsync();
            if (shelves.Count != bookRequest.ShelfIds.Count)
            {
                return BadRequest("Invalid shelfIds");
            }

            book.Shelves = shelves;
        }

        // Validate Date read and date finished
        if (book.DateReadUtc is not null && book.DateFinishedUtc is not null && book.DateReadUtc > book.DateFinishedUtc)
        {
            return BadRequest("Date read cannot be after date finished");
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new {id = book.Id}, BookResponse.FromBook(book));
    }

    /// <summary>
    /// Update book by id.
    /// </summary>
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] BookRequest bookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get user id from current token
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var bookToUpdate = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (bookToUpdate == null)
        {
            return NotFound();
        }

        bookToUpdate.ReplaceWithBookRequest(bookRequest);
        _context.Update(bookToUpdate);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete book by id.
    /// </summary>
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var bookToDelete = await _context.Books.Where(b => b.UserId == userId).FirstOrDefaultAsync(b => b.Id == id);
        if (bookToDelete == null)
        {
            return NotFound();
        }

        _context.Books.Remove(bookToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}