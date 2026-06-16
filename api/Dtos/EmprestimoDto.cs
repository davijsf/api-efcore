using Models;

namespace Dtos;

public class EmprestimoDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public int LivroId { get; set; }
    public string? LivroTitulo { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public EnuStatusEmprestimo Status { get; set; }
}

public class EmprestimoCreateDto
{
    public int UsuarioId { get; set; }
    public int LivroId { get; set; }
    public DateTime? DataEmprestimo { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
}
