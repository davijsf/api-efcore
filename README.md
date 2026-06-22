# Arquitetura do Projeto — API de Biblioteca (api-efcore)

## 1. Visão Geral

API REST local desenvolvida em **C# / ASP.NET Core (.NET 10)** para gerenciamento de uma biblioteca: usuários, perfis de acesso, livros, categorias e empréstimos. Persistência via **Entity Framework Core** com banco **SQLite**, e documentação/teste interativo via **Swagger (Swashbuckle)** + **OpenAPI** nativo do .NET.

## 2. Tecnologias e Pacotes

| Pacote | Função |
|---|---|
| `Microsoft.EntityFrameworkCore.Sqlite` | Provider de banco de dados (SQLite) |
| `Microsoft.EntityFrameworkCore.InMemory` | Provider em memória (Usado em testes) |
| `Microsoft.EntityFrameworkCore.Tools` / `.Design` | Suporte a Migrations via CLI (`dotnet ef`) |
| `Microsoft.AspNetCore.OpenApi` | Geração do documento OpenAPI nativo |
| `Swashbuckle.AspNetCore` | Swagger UI |
| `BCrypt.Net-Next` | Dependência adicionada para hashing de senha |

> Target Framework: `net10.0`, com `Nullable` e `ImplicitUsings` habilitados.

## 3. Estrutura de Diretórios

```
api-efcore/
├── api/
│   ├── Controllers/        # 5 controllers REST (Categoria, Emprestimo, Livro, Perfil, Usuario)
│   ├── Data/
│   │   └── AppDbContext.cs # DbContext + método PopulateTestData (seed)
│   ├── Dtos/                # DTOs de leitura/escrita + MappingExtensions
│   ├── Migrations/          # Histórico de schema (EF Core)
│   ├── Models/               # Entidades de domínio + enums
│   ├── Properties/
│   │   └── launchSettings.json  # Portas: http 5057 / https 7193
│   ├── Program.cs             # Configuração e pipeline da aplicação
│   ├── appsettings.json        # Connection string SQLite ("Data Source=labcore.db")
│   └── api.csproj
└── docs/
    └── guia-controllers.md     # Tutorial passo a passo para criar novos Controllers (exclusivo aos colaboradores, em tempo de desenvolvimento do projeto)
```

## 4. Modelo de Domínio

```
Categoria N ──── N Livro N ──── 1 Emprestimo N ──── 1 Usuario N ──── 1 Perfil
```

| Entidade | Campos principais | Relacionamento |
|---|---|---|
| **Categoria** | `Id`, `Nome`, `Descricao` | 1 Categoria → N Livros |
| **Livro** | `Id`, `Titulo`, `Autor`, `ISBN`, `AnoPublicacao`, `QuantidadeDisponivel`, `CategoriaId` | N:1 com Categoria; N:1 com Emprestimo |
| **Perfil** | `Id`, `Nivel` (`EnuNivelAcesso`) | 1 Perfil → N Usuários |
| **Usuario** | `Id`, `Nome`, `Email`, `SenhaHash`, `PerfilId` | N:1 com Perfil; N:1 com Emprestimo |
| **Emprestimo** | `Id`, `UsuarioId`, `LivroId`, `DataEmprestimo`, `DataPrevistaDevolucao`, `DataDevolucao` (nullable), `Status` (`EnuStatusEmprestimo`) | N:1 com Usuario; N:1 com Livro |

### Enums
- **`EnuNivelAcesso`**: `Admin`, `Bibliotecario`, `Leitor`
- **`EnuStatusEmprestimo`**: `Ativo`, `Devolvido`, `Atrasado`

> Observação: as migrations incluem uma chamada `AddProduto`, mas não há atualmente nenhuma entidade `Produto` no código — é resíduo de uma versão anterior do projeto.

## 5. Camadas da Aplicação

```
HTTP Request
     │
     ▼
Controllers  ─── recebe/valida a requisição, contém as regras de negócio
     │
     ▼
Dtos + MappingExtensions  ─── converte Model ⇄ Dto (CreateDto na entrada, Dto na saída)
     │
     ▼
Models  ─── entidades de domínio (mapeadas para tabelas)
     │
     ▼
AppDbContext (EF Core)  ─── DbSets, tracking, SaveChanges
     │
     ▼
SQLite (labcore.db)
```

### Padrão de DTO adotado
Cada entidade tem **dois DTOs**:
- `<Entidade>Dto` — usado nas respostas (saída), inclui dados "achatados" de relacionamentos (ex: `LivroDto.CategoriaNome`, `EmprestimoDto.UsuarioNome`/`LivroTitulo`)
- `<Entidade>CreateDto` — usado na entrada (POST/PUT), contendo apenas os campos que o cliente pode enviar

A conversão entre Model e Dto é centralizada em `MappingExtensions.cs`, evitando lógica de mapeamento duplicada nos controllers.

## 6. Regras de Negócio Implementadas

### `UsuarioController`
- Valida se o `PerfilId` informado existe antes de criar o usuário.
- A senha é "hasheada" com `BCrypt.Net-Next`.
```
usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
```
- Na atualização (`PUT`), a senha só é re-hasheada se uma nova senha for enviada.

### `LivroController`
- Valida se a `CategoriaId` informada existe antes de criar/atualizar um livro.

### `EmprestimoController` (contém a lógica mais rica do projeto)
- **Criar empréstimo (`POST`)**: verifica se o livro existe e se há exemplares disponíveis (`QuantidadeDisponivel > 0`); verifica se o usuário existe; decrementa `QuantidadeDisponivel` do livro; define `Status = Ativo`.
- **Devolver (`PATCH /api/emprestimo/{id}/devolver`)**: endpoint customizado (fora do CRUD padrão) que marca `DataDevolucao`, muda `Status` para `Devolvido` e devolve o exemplar ao estoque (`QuantidadeDisponivel++`). Bloqueia devolução duplicada.
- **Atrasados (`GET /api/emprestimo/atrasados`)**: endpoint customizado que lista empréstimos com `Status == Ativo` e `DataPrevistaDevolucao` no passado. *Atenção: este endpoint não atualiza o `Status` para `Atrasado` no banco — apenas filtra os ativos vencidos em tempo de consulta.*

### `CategoriaController` e `PerfilController`
- CRUD simples, sem regras de negócio adicionais.

## 7. Inicialização (`Program.cs`)

1. Lê a connection string `SqliteConnection` do `appsettings.json` (`Data Source=labcore.db`).
2. Registra `AppDbContext` com o provider SQLite.
3. Registra Controllers, OpenAPI nativo (`AddOpenApi`) e Swagger (`AddSwaggerGen`).
4. Configura uma política de CORS chamada `AllowReactApp` — **atenção**: o código define origens específicas (`localhost:5173`).
5. **Apenas em ambiente de Desenvolvimento**: habilita Swagger UI (`/swagger`) e **reseta o banco de dados a cada inicialização**, repopulando com dados de teste fixos (`AppDbContext.PopulateTestData()` — apaga todas as tabelas e insere 2 categorias, 3 livros, 3 perfis, 3 usuários e 3 empréstimos de exemplo).
6. Pipeline final: HTTPS redirect → CORS → Autorização → Controllers.

## 8. Como Executar Localmente

```bash
cd api
dotnet watch run --launch-profile https
```
- HTTP: `http://localhost:5057`
- HTTPS: `https://localhost:7193`
- Swagger UI: `https://localhost:7193/swagger`

Como o banco é resetado a cada execução em desenvolvimento, não é necessário rodar `dotnet ef database update` manualmente para testar — os dados de exemplo já são recriados no startup.

## 9. Documentação Complementar

Este repositório já conta com `docs/guia-controllers.md`, um tutorial passo a passo (Model → DbContext → Migration → Controller → Swagger) voltado para colaboradores que vão **adicionar novas entidades** ao projeto. Este documento (`README.md`) complementa esse guia oferecendo uma visão geral da arquitetura e das regras de negócio já implementadas.
