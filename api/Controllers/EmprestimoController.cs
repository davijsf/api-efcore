using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Dtos;

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
    public async Task<ActionResult<IEnumerable<EmprestimoDto>>> GetAll()
    {
        var emprestimos = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .ToListAsync();

        return Ok(emprestimos.Select(e => e.ToDto()));
    }

    // GET: api/emprestimo/1
    [HttpGet("{id}")]
    public async Task<ActionResult<EmprestimoDto>> GetById(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprestimo == null) return NotFound();
        return Ok(emprestimo.ToDto());
    }

    // POST: api/emprestimo
    [HttpPost]
    public async Task<ActionResult<EmprestimoDto>> Create(EmprestimoCreateDto dto)
    {
        // Verifica se o Livro existe e se tem quantidade disponível (> 0)
        var livro = await _context.Livros.FindAsync(dto.LivroId);
        if (livro == null) return NotFound("Livro não encontrado.");
        if (livro.QuantidadeDisponivel <= 0) return BadRequest("Livro indisponível para empréstimo.");
        
        // Verifica se o usuário existe
        var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
        if (usuario == null) return NotFound("Usuário não encontrado.");

        var emprestimo = dto.ToModel();
        emprestimo.Status = EnuStatusEmprestimo.Ativo;

        // Decrementa [QuantidadeDisponivel] do Livro:
        livro.QuantidadeDisponivel--;
        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();

        await _context.Entry(emprestimo).Reference(e => e.Livro).LoadAsync();
        await _context.Entry(emprestimo).Reference(e => e.Usuario).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = emprestimo.Id }, emprestimo.ToDto());
    }

    // PATCH: api/emprestimo/1/devolver
    [HttpPatch("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {  
        // Faz a busca por ID [Empréstimos]
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprestimo == null) return NotFound();
        if (emprestimo.Status == EnuStatusEmprestimo.Devolvido)
            return BadRequest("Este empréstimo já foi devolvido.");

        // Marca a {DataDevolucao}
        // Define Status = Ativo
        // Devolve o Livro ao estoque [QuantidadeDisponivel++]
        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = EnuStatusEmprestimo.Devolvido;
        emprestimo.Livro!.QuantidadeDisponivel++;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/emprestimo/atrasados
    [HttpGet("atrasados")]
    public async Task<ActionResult<IEnumerable<EmprestimoDto>>> GetAtrasados()
    {
        // retorna os Empréstimos atrasados
        var hoje = DateTime.UtcNow;

        var atrasados = await _context.Emprestimos
            .Include(e => e.Usuario)
            .Include(e => e.Livro)
            .Where(e => e.Status == EnuStatusEmprestimo.Ativo && e.DataPrevistaDevolucao < hoje)
            .ToListAsync();

        return Ok(atrasados.Select(e => e.ToDto()));
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