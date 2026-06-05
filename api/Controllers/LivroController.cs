using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

[ApiController]
[Route("api/[controller]")]
public class LivroController : ControllerBase
{
    
    private readonly AppDbContext _context;

    public LivroController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/livro
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Livro>>> GetAll()
    {
        return await _context.Livros.ToListAsync();
    }

    // GET: api/livro/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> GetById(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        return livro;
    }

    // POST: api/livro
    [HttpPost]
    public async Task<ActionResult<Livro>> Create(Livro livro)
    {
        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new {id = livro.Id}, livro);
    }

    // PUT: api/livro/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Livro livro)
    {
        if (id != livro.Id) return BadRequest();
        _context.Entry(livro).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/livro/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        _context.Livros.Remove(livro);
        return NoContent();
    }
}