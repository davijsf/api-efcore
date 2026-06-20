namespace Models;

public class Perfil
{
    public int Id { get; set; }
    public EnuNivelAcesso Nivel { get; set; }
    public List<Usuario> Usuarios { get; set; } = new();
}