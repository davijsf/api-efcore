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
        Console.WriteLine("Apagando dados de teste...");
        
        Emprestimos.RemoveRange(Emprestimos.ToList());
        SaveChanges();
        
        Usuarios.RemoveRange(Usuarios.ToList());
        Livros.RemoveRange(Livros.ToList());
        SaveChanges();
        
        Categorias.RemoveRange(Categorias.ToList());
        Perfis.RemoveRange(Perfis.ToList());
        SaveChanges();

        Console.WriteLine("Adicionar dados de teste...");

        // Categorias: 
        var cat1 = new Categoria { Nome = "Ficção" };
        var cat2 = new Categoria { Nome = "Aventura" };
        var cat3 = new Categoria { Nome = "Terror" };
        var cat4 = new Categoria { Nome = "Ação" };

        Categorias.AddRange(cat1, cat2, cat3, cat4);
        SaveChanges(); // Salva para gerar os Ids
        

        // livros
        var livro1 = new Livro { Titulo = "O pequeno príncipe",    Autor = "Davi de Dó",    AnoPublicacao = 2000, 
            QuantidadeDisponivel = 150};
        livro1.Categorias.AddRange(cat1, cat2);
        
        var livro2 = new Livro { Titulo = "As aventuras de Tintin", Autor = "Lusiane de Lá", AnoPublicacao = 1998,
            QuantidadeDisponivel = 10};
        livro2.Categorias.AddRange(cat1, cat2, cat3);

        var livro3 = new Livro { Titulo = "As crônicas de nárnia",  Autor = "Carlos de Si",  AnoPublicacao = 2015,
            QuantidadeDisponivel = 15};
        livro3.Categorias.AddRange(cat1, cat2, cat4);

        // Livro com apenas uma categoria, adiciona-se o id
        var livro4 = new Livro { Titulo = "Rambo III", Autor = "Jairo Albuquerque", AnoPublicacao = 1995,
            QuantidadeDisponivel = 60, CategoriaId = cat4.Id};
        livro4.Categorias.Add(cat4);
        
        Livros.AddRange(livro1, livro2, livro3, livro4);
        SaveChanges();

        // Perfis
        var p1 = new Perfil { Nivel = Enum.Parse<EnuNivelAcesso>("Admin")};
        var p2 = new Perfil { Nivel = Enum.Parse<EnuNivelAcesso>("Bibliotecario")};
        var p3 = new Perfil { Nivel = Enum.Parse<EnuNivelAcesso>("Leitor")};
        Perfis.AddRange(p1, p2, p3);
        SaveChanges();

        // Usuarios
        var usr1 = new Usuario { Email = "joao@email.com", Nome = "João", Perfil = p3, 
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123")};

        var usr2 = new Usuario { Email = "carlos@email.com", Nome = "Carlos", Perfil = p2, 
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("s1234")};
       
        var usr3 = new Usuario { Email = "lusiane@email.com", Nome = "Lusiane", Perfil = p3, 
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("s1234")};
        
        Usuarios.AddRange(usr1, usr2, usr3);
        SaveChanges();

        // Empréstimos
        var emprestimo1 = new Emprestimo
        {
            UsuarioId             = usr1.Id,
            LivroId               = livro1.Id,
            DataEmprestimo        = DateTime.Now.AddDays(-10),
            DataPrevistaDevolucao = DateTime.Now.AddDays(-3),
            DataDevolucao         = DateTime.Now.AddDays(-5), // Devolvido antes do prazo
            Status                = EnuStatusEmprestimo.Devolvido
        };

        var emprestimo2 = new Emprestimo
        {
            UsuarioId             = usr2.Id,
            LivroId               = livro2.Id,
            DataEmprestimo        = DateTime.Now.AddDays(-5),
            DataPrevistaDevolucao = DateTime.Now.AddDays(2),
            DataDevolucao         = null, // Ainda não devolvido
            Status                = EnuStatusEmprestimo.Ativo
        };

        var emprestimo3 = new Emprestimo
        {
            UsuarioId             = usr3.Id,
            LivroId               = livro3.Id,
            DataEmprestimo        = DateTime.Now.AddDays(-20),
            DataPrevistaDevolucao = DateTime.Now.AddDays(-10),
            DataDevolucao         = null, // Atrasado, não devolvido
            Status                = EnuStatusEmprestimo.Atrasado
        };
        Emprestimos.AddRange(emprestimo1, emprestimo2, emprestimo3);
        SaveChanges();
    }
}