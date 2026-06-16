using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Dtos;

[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerfilController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/perfil
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PerfilDto>>> GetAll()
    {
        var perfis = await _context.Perfis.ToListAsync();
        return Ok(perfis.Select(p => p.ToDto()));
    }

    // GET: api/perfil/1
    [HttpGet("{id}")]
    public async Task<ActionResult<PerfilDto>> GetById(int id)
    {
        var perfil = await _context.Perfis.FindAsync(id);
        if (perfil == null) return NotFound();
        return Ok(perfil.ToDto());
    }

    // POST: api/perfil
    [HttpPost]
    public async Task<ActionResult<PerfilDto>> Create(PerfilCreateDto dto)
    {
        var perfil = dto.ToModel();
        _context.Perfis.Add(perfil);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = perfil.Id }, perfil.ToDto());
    }

    // PUT: api/perfil/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PerfilCreateDto dto)
    {
        var perfil = await _context.Perfis.FindAsync(id);
        if (perfil == null) return NotFound();

        perfil.Nivel = dto.Nivel;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/perfil/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _context.Perfis.FindAsync(id);
        if (perfil == null) return NotFound();

        _context.Perfis.Remove(perfil);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}