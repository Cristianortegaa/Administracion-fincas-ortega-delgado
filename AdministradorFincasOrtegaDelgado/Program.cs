using System.Text;
using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.Helpers;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// En desarrollo usa localhost:5000; en producción Railway asigna el puerto vía variable PORT
if (builder.Environment.IsDevelopment())
    builder.WebHost.UseUrls("http://localhost:5000");

// ── Base de datos ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// ── JWT Authentication ─────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// ── DI: Repositories & Services ───────────────────────────────────────────────
builder.Services.AddScoped<ISiniestroRepository, SiniestroRepository>();
builder.Services.AddScoped<ISiniestroService, SiniestroService>();
builder.Services.AddScoped<IIncidenciaRepository, IncidenciaRepository>();
builder.Services.AddScoped<IIncidenciaService, IncidenciaService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
builder.Services.AddScoped<IExpedienteService, ExpedienteService>();

// ── Backup ────────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<IBackupService, BackupService>();
builder.Services.AddHostedService<BackupHostedService>();

// ── Controllers & OpenAPI ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.AllowAnyOrigin()   // demo: cualquier origen (Vercel + localhost)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Migración automática + seed ───────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Seed: crear usuario admin por defecto si no existe ninguno
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Email        = "admin@fincas.com",
            Name         = "Administrador",
            PasswordHash = PasswordHasher.Hash("Admin2026!"),
            CreatedAt    = DateTime.UtcNow,
            Role         = "Admin",
        });
        db.SaveChanges();
    }
    else
    {
        // Garantizar que el admin tenga rol Admin (por si la BD existía antes de esta migración)
        var adminUser = db.Users.FirstOrDefault(u => u.Email == "admin@fincas.com");
        if (adminUser is not null && adminUser.Role != "Admin")
        {
            adminUser.Role = "Admin";
            db.SaveChanges();
        }
    }
}

// ── Pipeline ──────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
