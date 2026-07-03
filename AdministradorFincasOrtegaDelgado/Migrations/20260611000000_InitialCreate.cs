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
    [Migration("20260611000000_InitialCreate")]
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Siniestros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaSiniestro = table.Column<DateOnly>(type: "date", nullable: false),
                    NumeroCDA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Comunidad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UbicacionDanio = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DetallesSiniestro = table.Column<string>(type: "text", nullable: false),
                    CompaniaSeguros = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TelefonoCompania = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReferenciaSiniestro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Reparador = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siniestros", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Siniestros");
        }
    }
}
