using Microsoft.EntityFrameworkCore;
using Models;

// Classe que representa a sessão do banco de dados
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Adição das tabelas (entidades):
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Perfil> Perfis { get; set; }

    public void PopulateTestData()
    {
        Console.WriteLine("Apago dados de teste...");
        Livros.RemoveRange(Livros.ToList());
        Console.WriteLine("Adicionar dados de teste...");
        Livros.AddRange(
            new Livro { Titulo = "O pequeno príncipe", Autor = "Davi de Dó", AnoPublicacao = 2000, QuantidadeDisponivel = 150},
            new Livro { Titulo = "As aventuras de Tintin", Autor = "Lusiane de Lá", AnoPublicacao = 1998, QuantidadeDisponivel = 10},
            new Livro { Titulo = "As crônicas de nárnia", Autor = "Carlos de Si", AnoPublicacao = 2015, QuantidadeDisponivel = 15}
        );
        SaveChanges();
    }
}