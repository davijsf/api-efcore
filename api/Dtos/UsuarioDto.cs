using Models;

namespace Dtos;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PerfilId { get; set; }
    public EnuNivelAcesso? NivelPerfil { get; set; }
}

public class UsuarioCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public int PerfilId { get; set; }
}
