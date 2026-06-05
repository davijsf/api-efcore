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

var app = builder.Build();

// 4. Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // gera o documento OpenAPI
    app.UseSwagger(); // gera /swagger/v1/swagger.json
    app.UseSwaggerUI(); // gera a UI em /swagger
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
