using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    [Migration("20260709000001_AddComunidades")]
    public partial class AddComunidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comunidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroComunidad = table.Column<int>(type: "integer", nullable: true),
                    CIF = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CompaniaSeguros = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroPoliza = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TelefonoSeguro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunidades", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Comunidades");
        }
    }
}
