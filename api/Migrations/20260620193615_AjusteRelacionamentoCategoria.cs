using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRelacionamentoCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livros_Categorias_CategoriaId",
                table: "Livros");

            migrationBuilder.DropIndex(
                name: "IX_Livros_CategoriaId",
                table: "Livros");

            migrationBuilder.CreateTable(
                name: "CategoriaLivro",
                columns: table => new
                {
                    CategoriasId = table.Column<int>(type: "INTEGER", nullable: false),
                    LivrosId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaLivro", x => new { x.CategoriasId, x.LivrosId });
                    table.ForeignKey(
                        name: "FK_CategoriaLivro_Categorias_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriaLivro_Livros_LivrosId",
                        column: x => x.LivrosId,
                        principalTable: "Livros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaLivro_LivrosId",
                table: "CategoriaLivro",
                column: "LivrosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaLivro");

            migrationBuilder.CreateIndex(
                name: "IX_Livros_CategoriaId",
                table: "Livros",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livros_Categorias_CategoriaId",
                table: "Livros",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
