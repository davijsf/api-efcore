using Models;

namespace Dtos;

public static class MappingExtensions
{
    public static CategoriaDto ToDto(this Categoria c)
        => new CategoriaDto { Id = c.Id, Nome = c.Nome, Descricao = c.Descricao };

    public static Categoria ToModel(this CategoriaCreateDto d)
        => new Categoria { Nome = d.Nome, Descricao = d.Descricao };

    public static LivroDto ToDto(this Livro l)
        => new LivroDto
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Autor = l.Autor,
            ISBN = l.ISBN,
            AnoPublicacao = l.AnoPublicacao,
            QuantidadeDisponivel = l.QuantidadeDisponivel,
            CategoriaId = l.CategoriaId,
            CategoriaNome = l.Categorias.FirstOrDefault(c => c.Id == l.CategoriaId)?.Nome ?? "",
        };

    public static Livro ToModel(this LivroCreateDto d)
        => new Livro
        {
            Titulo = d.Titulo,
            Autor = d.Autor,
            ISBN = d.ISBN,
            AnoPublicacao = d.AnoPublicacao,
            QuantidadeDisponivel = d.QuantidadeDisponivel,
            CategoriaId = d.CategoriaId
        };

    public static PerfilDto ToDto(this Perfil p)
        => new PerfilDto { Id = p.Id, Nivel = p.Nivel };

    public static Perfil ToModel(this PerfilCreateDto d)
        => new Perfil { Nivel = d.Nivel };

    public static UsuarioDto ToDto(this Usuario u)
        => new UsuarioDto { Id = u.Id, Nome = u.Nome, Email = u.Email, PerfilId = u.PerfilId, NivelPerfil = u.Perfil?.Nivel };

    public static Usuario ToModel(this UsuarioCreateDto d)
        => new Usuario { Nome = d.Nome, Email = d.Email, SenhaHash = string.Empty, PerfilId = d.PerfilId };

    public static EmprestimoDto ToDto(this Emprestimo e)
        => new EmprestimoDto
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            UsuarioNome = e.Usuario?.Nome,
            LivroId = e.LivroId,
            LivroTitulo = e.Livro?.Titulo,
            DataEmprestimo = e.DataEmprestimo,
            DataPrevistaDevolucao = e.DataPrevistaDevolucao,
            DataDevolucao = e.DataDevolucao,
            Status = e.Status
        };

    public static Emprestimo ToModel(this EmprestimoCreateDto d)
        => new Emprestimo
        {
            UsuarioId = d.UsuarioId,
            LivroId = d.LivroId,
            DataEmprestimo = d.DataEmprestimo ?? DateTime.Now,
            DataPrevistaDevolucao = d.DataPrevistaDevolucao,
            Status = EnuStatusEmprestimo.Ativo
        };
}
