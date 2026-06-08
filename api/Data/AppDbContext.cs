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
}