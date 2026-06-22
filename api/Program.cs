using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

// Cria o builder da aplicação
var builder = WebApplication.CreateBuilder(args);

// 1.  Leitura da string de conexão.
string? stcnn = builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("String de conexão 'SqliteConnection' não encontrada.");

// 2. Registrar o DBContext com SQlite
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(stcnn));

// 3. Serviços da aplicação
builder.Services.AddControllers(); // [ApiController], [HttpGet], etc.
builder.Services.AddEndpointsApiExplorer(); // Necessário para o Swagger

// Registra o gerador de documentação Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Biblioteca",
        Version = "v1",
        Description = "API para gerenciamento de livros, usuários e empréstimo de uma biblioteca."
    });
}); 

// Registra o suporte nativo do .NET para geração de documentos OpenAPI
builder.Services.AddOpenApi();


// Adicionar politica de CORS (Cross-Origin Resource Sharing)
// É um mecanismo de segurança implementado pelos navegadores
// que controla se uma aplicação web em um domínio (por exemplo,
// http://frontend.com.br) pode fazer requisições para um servidor
// em outro domínio http://api.backend.com)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
                )
                  .AllowAnyHeader() // aceita qualquer cabeçalho Http na requisição
                  .AllowAnyMethod(); // aceita qualquer verbo Http(Get, Post, Put, Delete, etc).

        });
});

// O builder "finaliza" a config 
// e gera o objeto app (WebApplication) 
var app = builder.Build();

// 4. Pipeline
// PS: Só roda se a aplicação estiver rodando em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())    
{
    app.MapOpenApi(); // gera o documento OpenAPI
    app.UseSwagger(); // gera /swagger/v1/swagger.json
    app.UseSwaggerUI(); // gera a UI em /swagger
    
    // Cria um escopo da injeção de dependência
    using ( var scope = app.Services.CreateScope()) 
    {   
        // Pega a instância do AppDbContext desse escopo
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Evita um erro ao popular os dados
        try 
        {
            // Popula o Banco
            context.PopulateTestData();
        } 
        catch (Exception ex)
        {
            Console.WriteLine(" Erro ao popular dados de teste: ");
            Console.WriteLine(ex.Message);
        }
    }
}

// Middlewares finais
// Ps: A ordem dos middlewares importa
app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS
app.UseCors("AllowReactApp");
app.UseAuthorization(); // Autoriza o middleware de autorização --
                        //  verifica se o usuário tem permissão para acessar determinado endpoint
app.MapControllers(); // mapeia as rotas definidas pelos Controllers ([Route], [HttpGet], etc.)
app.Run(); // inicia a aplicação 

