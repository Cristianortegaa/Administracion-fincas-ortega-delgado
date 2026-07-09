using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    [Migration("20260709000002_AddNumeroPolizaToExpediente")]
    public partial class AddNumeroPolizaToExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroPoliza",
                table: "Expedientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroPoliza",
                table: "Expedientes");
        }
    }
}
