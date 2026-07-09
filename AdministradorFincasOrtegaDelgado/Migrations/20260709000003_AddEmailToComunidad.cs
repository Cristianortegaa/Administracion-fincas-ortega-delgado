using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    [Migration("20260709000003_AddEmailToComunidad")]
    public partial class AddEmailToComunidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Comunidades",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Email", table: "Comunidades");
        }
    }
}
