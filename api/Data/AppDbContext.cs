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
}