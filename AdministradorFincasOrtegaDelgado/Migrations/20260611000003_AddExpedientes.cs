using System;
using AdministradorFincasOrtegaDelgado.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260611000003_AddExpedientes")]
    public partial class AddExpedientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expedientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Comunidad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Ubicacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    TipoIncidencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Reparador = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NumeroCDA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CompaniaSeguros = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TelefonoCompania = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReferenciaSiniestro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreadoPorId = table.Column<int>(type: "integer", nullable: true),
                    CreadoPorNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificadoPorId = table.Column<int>(type: "integer", nullable: true),
                    ModificadoPorNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expedientes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Expedientes");
        }
    }
}
