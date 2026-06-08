using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTodasTabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Perfis");

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Perfis",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Perfis");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Perfis",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
