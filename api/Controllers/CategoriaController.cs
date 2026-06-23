using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dtos;

[ApiController] 
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    // Injeção de dependência
    // AppDbContext é recebido via construtor 
    // _context é guardado num campo privado e readonly
    // para ser usado nos métodos da classe
    private readonly AppDbContext _context;
    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categoria
    // Listar todos
    [HttpGet]
    // Task<ActionResult<IEnumerable<CategoriaDto>>>: o tipo do retorno
    // ActionResult<T>: permite devolver o objeto T ou um código HTTP(NotFound(), etc).
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        var categorias = await _context.Categorias.ToListAsync();
        return Ok(categorias.Select(c => c.ToDto())); // Trasnforma cada entidade em um DTO
    }

    // GET: api/categoria/1
    // Buscar por id
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        // busca pela chave primária
        // se não existir, retorna 404 Not Found()
        // senão, converte pra DTO e retorna 200 Ok
        var categoria = await _context.Categorias.FindAsync(id); 
        if (categoria == null) return NotFound();
        return Ok(categoria.ToDto());
    }

    // POST: api/categoria
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Create(CategoriaCreateDto dto)
    {
        
        // tranforma o DTO de entrada em uma entidade
        var categoria = dto.ToModel();
        // marca a entidade para ser inserida (ainda não vai ao banco)
        _context.Categorias.Add(categoria);
        // aqui o insert é executado no banco
        await _context.SaveChangesAsync();
        // retorna 201 Created
        return CreatedAtAction(nameof(GetById), 
        new { id = categoria.Id }, categoria.ToDto());
    }

    // PUT: api/categoria/1
    // atualizar
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoriaCreateDto dto)
    {
        // busca a categoria existente; se não existir, 404
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        // atualiza manualmente os campos da entidade
        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;

        // gera um update e retorna 204 No Content
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/categoria/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // busca, verifica a existência (404 se não achar)
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();
        
        // marca para exclusão
        _context.Categorias.Remove(categoria);
        // executa o DELETE no banco
        await _context.SaveChangesAsync();
        return NoContent();
    }
}