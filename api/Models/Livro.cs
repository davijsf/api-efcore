namespace Models;

public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public int QuantidadeDisponivel { get; set; }

    // Pode ser nulo, no caso, se o Livro tiver mais de uma categoria: 
    public int ? CategoriaId { get; set; }
    
    public List<Categoria> Categorias { get; set; } = new();
}