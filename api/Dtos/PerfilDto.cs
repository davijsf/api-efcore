using Models;

namespace Dtos;

public class PerfilDto
{
    public int Id { get; set; }
    public EnuNivelAcesso Nivel { get; set; }
}

public class PerfilCreateDto
{
    public EnuNivelAcesso Nivel { get; set; }
}
