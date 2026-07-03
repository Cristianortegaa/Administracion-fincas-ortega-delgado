using AdministradorFincasOrtegaDelgado.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    /// <inheritdoc />
    [Migration("20260615000001_ReparadoresArray")]
    [DbContext(typeof(AdministradorFincasOrtegaDelgado.Data.ApplicationDbContext))]
    public partial class ReparadoresArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Añadir columna nueva (text, JSON)
            migrationBuilder.AddColumn<string>(
                name: "Reparadores",
                table: "Expedientes",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            // 2. Migrar datos: envolver el valor existente en un array JSON
            migrationBuilder.Sql(@"
                UPDATE ""Expedientes""
                SET ""Reparadores"" =
                    CASE
                        WHEN ""Reparador"" IS NULL OR trim(""Reparador"") = ''
                        THEN '[]'
                        ELSE json_build_array(""Reparador"")::text
                    END
            ");

            // 3. Eliminar columna antigua
            migrationBuilder.DropColumn(
                name: "Reparador",
                table: "Expedientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Invertir: añadir columna string, extraer primer elemento, eliminar JSON
            migrationBuilder.AddColumn<string>(
                name: "Reparador",
                table: "Expedientes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""Expedientes""
                SET ""Reparador"" = COALESCE(
                    (SELECT value FROM json_array_elements_text(""Reparadores""::json) LIMIT 1),
                    ''
                )
            ");

            migrationBuilder.DropColumn(
                name: "Reparadores",
                table: "Expedientes");
        }
    }
}
