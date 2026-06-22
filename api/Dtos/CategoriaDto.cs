namespace Dtos;

public class CategoriaDto // SAÍDA (O QUE O SISTEMA DEVOLVE PARA O CLIENTE)
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public class CategoriaCreateDto // ENTRADA (O QUE O CLIENTE ENVIA)
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
