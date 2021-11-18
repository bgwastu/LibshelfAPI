using ImageMagick;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Features.Books;

[Route("/api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly LibshelfContext _context;

    public BooksController(LibshelfContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books.Select(BookResponse.From).ToList());
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        var bookResponse = BookResponse.From(book);

        return Ok(bookResponse);
    }

    [HttpGet("{id:Guid}/shelves")]
    public async Task<IActionResult> GetShelves(Guid id)
    {
        var book = await _context.Books.Include(b => b.Shelves).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return Ok(book.Shelves);
    }
    
    /// <summary>
    /// Upload cover image and convert into .webp
    /// </summary>
    [HttpPost("uploadCover")]
    public async Task<IActionResult> UploadCover([FromForm] IFormFile file)
    {
        var fileName = Guid.NewGuid();
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images",
            fileName + Path.GetExtension(file.FileName));
        var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images",
            fileName + ".webp");


        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        using (var image = new MagickImage(filePath))
        {
            await image.WriteAsync(newFilePath);
            System.IO.File.Delete(filePath);
        }
        
        return Ok(new Dictionary<string, string>
        {
            {"fileName", "images/" + fileName + ".webp"}
        });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] BookRequest bookRequest)
    {
        var book = Book.From(bookRequest);

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


        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new {id = book.Id}, book);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] BookRequest bookRequest)
    {
        var bookToUpdate = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (bookToUpdate == null)
        {
            return NotFound();
        }

        var book = Book.From(bookRequest);
        book.Id = bookToUpdate.Id;
        _context.Update(book);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var bookToDelete = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (bookToDelete == null)
        {
            return NotFound();
        }

        _context.Books.Remove(bookToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}