using System.Text.Json;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Siniestro> Siniestros => Set<Siniestro>();
    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Expediente> Expedientes => Set<Expediente>();
    public DbSet<Comunidad> Comunidades => Set<Comunidad>();

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Deserializes a JSON array of DescripcionEntrada.
    /// Falls back gracefully: if the stored value is plain text (legacy), wraps it as a single entry.
    /// </summary>
    private static List<DescripcionEntrada> DeserializeDescripciones(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return [];
        try
        {
            var result = JsonSerializer.Deserialize<List<DescripcionEntrada>>(v, JsonOpts);
            if (result is { Count: > 0 }) return result;
        }
        catch { /* not JSON — treat as legacy plain text */ }

        // Legacy plain text: wrap in a single entry with unknown author
        return [new DescripcionEntrada { Texto = v, Fecha = DateTime.MinValue, Usuario = "" }];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Siniestro>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.NumeroCDA).HasMaxLength(50);
            entity.Property(s => s.Comunidad).HasMaxLength(200);
            entity.Property(s => s.UbicacionDanio).HasMaxLength(200);
            entity.Property(s => s.CompaniaSeguros).HasMaxLength(200);
            entity.Property(s => s.TelefonoCompania).HasMaxLength(50);
            entity.Property(s => s.ReferenciaSiniestro).HasMaxLength(100);
            entity.Property(s => s.Reparador).HasMaxLength(300);
            entity.Property(s => s.Estado).HasConversion<string>();
            entity.Property(s => s.DetallesSiniestro)
                .HasColumnType("text")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => DeserializeDescripciones(v)
                );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).HasColumnType("text").IsRequired();
            entity.Property(u => u.Name).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(20).IsRequired().HasDefaultValue("Worker");
        });

        modelBuilder.Entity<Incidencia>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Comunidad).HasMaxLength(200);
            entity.Property(i => i.Ubicacion).HasMaxLength(200);
            entity.Property(i => i.Tipo).HasMaxLength(100);
            entity.Property(i => i.Reparador).HasMaxLength(300);
            entity.Property(i => i.Observaciones).HasColumnType("text");
            entity.Property(i => i.Estado).HasConversion<string>();
            entity.Property(i => i.Descripcion)
                .HasColumnType("text")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => DeserializeDescripciones(v)
                );
        });

        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Comunidad).HasMaxLength(200);
            entity.Property(e => e.NumeroComunidad);
            entity.Property(e => e.Ubicacion).HasMaxLength(200);
            entity.Property(e => e.TipoIncidencia).HasMaxLength(100);
            entity.Property(e => e.Reparadores)
                .HasColumnType("text")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                );
            entity.Property(e => e.Observaciones).HasColumnType("text");
            entity.Property(e => e.NumeroCDA).HasMaxLength(50);
            entity.Property(e => e.CompaniaSeguros).HasMaxLength(200);
            entity.Property(e => e.TelefonoCompania).HasMaxLength(50);
            entity.Property(e => e.ReferenciaSiniestro).HasMaxLength(100);
            entity.Property(e => e.NumeroPoliza).HasMaxLength(100);
            entity.Property(e => e.CreadoPorNombre).HasMaxLength(200);
            entity.Property(e => e.ModificadoPorNombre).HasMaxLength(200);
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOpts),
                    v => DeserializeDescripciones(v)
                );
        });

        modelBuilder.Entity<Comunidad>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(c => c.CIF).HasMaxLength(20);
            entity.Property(c => c.Direccion).HasMaxLength(300);
            entity.Property(c => c.CompaniaSeguros).HasMaxLength(200);
            entity.Property(c => c.NumeroPoliza).HasMaxLength(100);
            entity.Property(c => c.TelefonoSeguro).HasMaxLength(50);
        });
    }
}
