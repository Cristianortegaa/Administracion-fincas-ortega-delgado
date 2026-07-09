using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    [Migration("20260709000004_AddEmailSeguroToExpediente")]
    public partial class AddEmailSeguroToExpediente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailSeguro",
                table: "Expedientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailSeguro",
                table: "Expedientes");
        }
    }
}
