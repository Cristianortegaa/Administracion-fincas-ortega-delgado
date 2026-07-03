using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    [Migration("20260630000001_AddNumeroComunidad")]
    public partial class AddNumeroComunidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroComunidad",
                table: "Expedientes",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroComunidad",
                table: "Expedientes");
        }
    }
}
