# API com C# + EF Core + SQLite
## Guia de Controllers para Colaboradores

---

## 1. Estrutura do Projeto

```
api/
├── Controllers/        ← onde ficam os controllers
├── Data/
│   └── AppDbContext.cs  ← configuração do banco
├── Models/
│   └── SuaEntidade.cs   ← modelo/tabela
├── appsettings.json     ← string de conexão
└── Program.cs           ← ponto de entrada
```

---

## 2. O que é um Controller?

Um Controller recebe as requisições HTTP dos clientes (Swagger, Postman, frontend) e devolve respostas. Cada método corresponde a uma operação no banco de dados.

| Método HTTP | Endpoint           | Ação           | Retorno esperado  |
|-------------|--------------------|----------------|-------------------|
| GET         | /api/livro         | Listar todos   | 200 OK + lista    |
| GET         | /api/livro/{id}    | Buscar por ID  | 200 OK ou 404     |
| POST        | /api/livro         | Criar novo     | 201 Created       |
| PUT         | /api/livro/{id}    | Atualizar      | 204 No Content    |
| DELETE      | /api/livro/{id}    | Deletar        | 204 No Content    |

---

## 3. Passo a Passo

### Passo 1 — Criar o Model

Crie um arquivo em `Models/`. Cada propriedade vira uma coluna no banco. O `Id` é reconhecido automaticamente como chave primária.

```csharp
// Models/Livro.cs
public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor  { get; set; } = string.Empty;
    public int Ano       { get; set; }
}
```

---

### Passo 2 — Registrar no AppDbContext

Abra `Data/AppDbContext.cs` e adicione um `DbSet` para o novo Model:

```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Livro> Livros { get; set; }  // <- adicionar aqui
}
```

---

### Passo 3 — Criar e aplicar a Migration

Toda vez que você alterar um Model ou adicionar um novo `DbSet`, precisa criar uma migration para atualizar o banco:

```bash
# Criar a migration (escolha um nome descritivo)
dotnet ef migrations add CriarTabelaLivro

# Aplicar ao banco de dados
dotnet ef database update
```

> Se não tiver o `dotnet-ef` instalado:
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet add package Microsoft.EntityFrameworkCore.Design
> ```

---

### Passo 4 — Criar o Controller

Crie um arquivo em `Controllers/`. O nome **deve** terminar em `Controller`:

```csharp
// Controllers/LivroController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class LivroController : ControllerBase
{
    private readonly AppDbContext _context;

    // Injeção de dependência — o EF Core injeta o contexto automaticamente
    public LivroController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/livro — retorna todos os registros
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Livro>>> GetAll()
    {
        return await _context.Livros.ToListAsync();
    }

    // GET: api/livro/1 — retorna um registro pelo ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> GetById(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        return livro;
    }

    // POST: api/livro — cria um novo registro
    [HttpPost]
    public async Task<ActionResult<Livro>> Create(Livro livro)
    {
        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = livro.Id }, livro);
    }

    // PUT: api/livro/1 — atualiza um registro existente
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Livro livro)
    {
        if (id != livro.Id) return BadRequest();
        _context.Entry(livro).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/livro/1 — deleta um registro
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

> **🚨 Regra importante:** Todo método do Controller **deve** ter um atributo HTTP (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`). Sem isso, o Swagger retorna erro 500.

---

### Passo 5 — Rodar a API e testar no Swagger

**1.** Rode a aplicação no terminal:

```bash
dotnet watch run --launch-profile https
```

O `watch` faz a API reiniciar automaticamente a cada alteração no código.

**2.** Observe no terminal a porta gerada:

```
Now listening on: https://localhost:7193
Now listening on: http://localhost:5057
```

**3.** Acesse no navegador:

```
https://localhost:7193/swagger
```

> A porta muda a cada projeto. Sempre verifique o terminal. Ela também está em `Properties/launchSettings.json` na chave `applicationUrl`.

**4.** Na interface do Swagger, expanda o endpoint desejado, clique em **Try it out** e depois em **Execute**.

---

## 4. Checklist Rápido

Antes de rodar, confirme:

- [ ] Model criado em `Models/`
- [ ] `DbSet` adicionado no `AppDbContext`
- [ ] Migration criada e aplicada (`dotnet ef database update`)
- [ ] Controller criado em `Controllers/` com nome terminando em `Controller`
- [ ] Todos os métodos têm atributo HTTP (`[HttpGet]`, `[HttpPost]`, etc.)
- [ ] `appsettings.json` possui a chave `SqliteConnection`

---

## 5. Erros Comuns

| Erro | Solução |
|------|---------|
| Swagger retorna erro 500 | Algum método do Controller está sem atributo HTTP |
| Página não encontrada no /swagger | Verifique a porta no terminal e use a URL correta |
| Tabela não existe no banco | Rode `dotnet ef migrations add Nome` e `dotnet ef database update` |
| Erro de conexão com SQLite | Verifique a chave `SqliteConnection` no `appsettings.json` |
| `dotnet ef` não encontrado | Instale com `dotnet tool install --global dotnet-ef` |
