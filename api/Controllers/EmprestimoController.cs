using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

[ApiController]
[Route("api/[controller]")]
public class EmprestimoController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmprestimoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/emprestimo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Emprestimo>>> GetAll()
    {
        return await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .ToListAsync();
    }

    // GET: api/emprestimo/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Emprestimo>> GetById(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprestimo == null) return NotFound();
        return emprestimo;
    }

    // POST: api/emprestimo
    [HttpPost]
    public async Task<ActionResult<Emprestimo>> Create(Emprestimo emprestimo)
    {
        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro == null) return NotFound("Livro não encontrado.");
        if (livro.QuantidadeDisponivel <= 0) return BadRequest("Livro indisponível para empréstimo.");

        emprestimo.DataEmprestimo = DateTime.UtcNow;
        emprestimo.Status = EnuStatusEmprestimo.Ativo;

        livro.QuantidadeDisponivel--;

        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = emprestimo.Id }, emprestimo);
    }

    // PATCH: api/emprestimo/1/devolver
    [HttpPatch("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprestimo == null) return NotFound();
        if (emprestimo.Status == EnuStatusEmprestimo.Devolvido)
            return BadRequest("Este empréstimo já foi devolvido.");

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = EnuStatusEmprestimo.Devolvido;

        emprestimo.Livro!.QuantidadeDisponivel++;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/emprestimo/atrasados
    [HttpGet("atrasados")]
    public async Task<ActionResult<IEnumerable<Emprestimo>>> GetAtrasados()
    {
        var hoje = DateTime.UtcNow;

        var atrasados = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .Where(e => e.Status == EnuStatusEmprestimo.Ativo && e.DataPrevistaDevolucao < hoje)
            .ToListAsync();

        return Ok(atrasados);
    }

    // DELETE: api/emprestimo/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var emprestimo = await _context.Emprestimos.FindAsync(id);
        if (emprestimo == null) return NotFound();

        _context.Emprestimos.Remove(emprestimo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}