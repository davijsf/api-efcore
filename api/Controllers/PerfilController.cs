using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

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
    public async Task<ActionResult<IEnumerable<Perfil>>> GetAll()
    {
        return await _context.Perfis.ToListAsync();
    }

    // GET: api/perfil/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Perfil>> GetById(int id)
    {
        var perfil = await _context.Perfis.FindAsync(id);
        if (perfil == null) return NotFound();
        return perfil;
    }

    // POST: api/perfil
    [HttpPost]
    public async Task<ActionResult<Perfil>> Create(Perfil perfil)
    {
        _context.Perfis.Add(perfil);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new {id = perfil.Id}, perfil);
    }

    // PUT: api/perfil/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Perfil perfil)
    {
        if (id != perfil.Id) return BadRequest();
        _context.Entry(perfil).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/perfil/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _context.Perfis.FindAsync(id);
        if ( perfil == null ) return NotFound();

        _context.Perfis.Remove(perfil);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}