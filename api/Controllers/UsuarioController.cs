using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Dtos;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _hasher;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
        _hasher = new PasswordHasher<Usuario>();
    }

    // GET: api/usuario
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Perfil)
            .ToListAsync();

        return Ok(usuarios.Select(u => u.ToDto()));
    }

    // GET: api/usuario/1
    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null) return NotFound();
        return Ok(usuario.ToDto());
    }

    // POST: api/usuario
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create(UsuarioCreateDto dto)
    {
        if (!await _context.Perfis.AnyAsync(p => p.Id == dto.PerfilId))
            return BadRequest("Perfil inválido.");

        var usuario = dto.ToModel();
        usuario.SenhaHash = _hasher.HashPassword(usuario, dto.Senha);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        await _context.Entry(usuario).Reference(u => u.Perfil).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario.ToDto());
    }

    // PUT: api/usuario/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UsuarioCreateDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        if (!await _context.Perfis.AnyAsync(p => p.Id == dto.PerfilId))
            return BadRequest("Perfil inválido.");

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.PerfilId = dto.PerfilId;

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            usuario.SenhaHash = _hasher.HashPassword(usuario, dto.Senha);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/usuario/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}