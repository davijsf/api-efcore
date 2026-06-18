using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dtos;

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
    public async Task<ActionResult<IEnumerable<LivroDto>>> GetAll()
    {
        var livros = await _context.Livros
            .Include(l => l.Categoria)
            .ToListAsync();

        return Ok(livros.Select(l => l.ToDto()));
    }

    // GET: api/livro/1
    [HttpGet("{id}")]
    public async Task<ActionResult<LivroDto>> GetById(int id)
    {
        var livro = await _context.Livros
            .Include(l => l.Categoria)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null) return NotFound();
        return Ok(livro.ToDto());
    }

    // POST: api/livro
    [HttpPost]
    public async Task<ActionResult<LivroDto>> Create(LivroCreateDto dto)
    {
        // Verifica se categoria existe:
        if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
            return BadRequest("Categoria inválida.");

        // Se existir, adiciona o Livro:
        var livro = dto.ToModel();
        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = livro.Id }, livro.ToDto());
    }

    // PUT: api/livro/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LivroCreateDto dto)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();

        if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
            return BadRequest("Categoria inválida.");

        livro.Titulo = dto.Titulo;
        livro.Autor = dto.Autor;
        livro.ISBN = dto.ISBN;
        livro.AnoPublicacao = dto.AnoPublicacao;
        livro.QuantidadeDisponivel = dto.QuantidadeDisponivel;
        livro.CategoriaId = dto.CategoriaId;

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
        await _context.SaveChangesAsync();
        return NoContent();
    }
}