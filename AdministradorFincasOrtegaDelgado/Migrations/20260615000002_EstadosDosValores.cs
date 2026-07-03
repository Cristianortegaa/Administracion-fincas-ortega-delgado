using AdministradorFincasOrtegaDelgado.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministradorFincasOrtegaDelgado.Migrations
{
    /// <summary>
    /// Simplifica los estados de Expediente a solo Abierto / Cerrado.
    /// EnProceso y Finalizado se convierten a Cerrado.
    /// </summary>
    [Migration("20260615000002_EstadosDosValores")]
    [DbContext(typeof(ApplicationDbContext))]
    public partial class EstadosDosValores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Expedientes""
                SET ""Estado"" = 'Cerrado'
                WHERE ""Estado"" IN ('EnProceso', 'Finalizado')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No podemos saber cuáles eran EnProceso vs Finalizado; dejamos como Abierto
            migrationBuilder.Sql(@"
                UPDATE ""Expedientes""
                SET ""Estado"" = 'Abierto'
                WHERE ""Estado"" = 'Cerrado'
            ");
        }
    }
}
