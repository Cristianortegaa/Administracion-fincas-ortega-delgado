using AdministradorFincasOrtegaDelgado.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260615000003_AddUserRole")]
public partial class AddUserRole : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Añadir columna Role con valor por defecto 'Worker'
        migrationBuilder.AddColumn<string>(
            name: "Role",
            table: "Users",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "Worker");

        // El usuario admin@fincas.com es el administrador
        migrationBuilder.Sql("""
            UPDATE "Users" SET "Role" = 'Admin'
            WHERE "Email" = 'admin@fincas.com';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Role", table: "Users");
    }
}
