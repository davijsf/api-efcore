using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1.  Leitura da string de conexão.
string? stcnn = builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("String de conexão 'SqliteConnection' não encontrada.");

// 2. Registrar o DBContext com SQlite
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(stcnn));

// 3. Serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necessário para o Swagger
builder.Services.AddSwaggerGen(); // Gera a UI interativa
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
                  .AllowAnyHeader()
                  .AllowAnyMethod();

        });
});


var app = builder.Build();

// 4. Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // gera o documento OpenAPI
    app.UseSwagger(); // gera /swagger/v1/swagger.json
    app.UseSwaggerUI(); // gera a UI em /swagger
    
    using ( var scope = app.Services.CreateScope()) {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.PopulateTestData();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.Run();
