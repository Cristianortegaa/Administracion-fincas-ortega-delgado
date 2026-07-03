# Administrador Fincas Ortega & Delgado — Instrucciones de Arranque

## Requisitos previos
- .NET 10 SDK
- Node.js 20+ / npm 10+
- PostgreSQL 15+ en ejecución

---

## 1. Base de datos PostgreSQL

Crea la base de datos (una sola vez):
```sql
CREATE DATABASE fincas_db;
```

Edita la cadena de conexión en:
`AdministradorFincasOrtegaDelgado/appsettings.json`

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=fincas_db;Username=postgres;Password=TU_PASSWORD"
```

---

## 2. Backend (.NET 10)

```bash
cd AdministradorFincasOrtegaDelgado

# Restaurar paquetes y arrancar (las migraciones se aplican automáticamente)
dotnet run
```

El backend arranca en `http://localhost:5000` (o `https://localhost:5001`).

> Las tablas se crean automáticamente la primera vez que arranca.

---

## 3. Frontend (Angular 21)

```bash
cd fincas-front

npm install
npm start
```

Abre `http://localhost:4200`

---

## Arquitectura del Backend

```
AdministradorFincasOrtegaDelgado/
├── Models/          → Entidades de dominio (Siniestro, EstadoSiniestro)
├── Data/            → ApplicationDbContext (EF Core)
├── DTOs/            → SiniestroDto, CreateSiniestroDto, UpdateSiniestroDto, SiniestroFilterDto
├── Mappers/         → SiniestroMapper (ToDto / ToModel / ApplyUpdate)
├── Repositories/    → ISiniestroRepository + SiniestroRepository
├── Services/        → ISiniestroService + SiniestroService
├── Controllers/     → SiniestroController (CRUD + filtros)
└── Migrations/      → Migración inicial de EF Core
```

## Arquitectura del Frontend

```
fincas-front/src/app/
├── core/
│   ├── models/      → siniestro.model.ts (interfaces TypeScript)
│   └── services/    → siniestro.service.ts, toast.service.ts
├── features/
│   ├── dashboard/   → Vista resumen con KPIs
│   └── siniestros/
│       ├── siniestros-list/   → Tabla principal con filtros y búsqueda
│       ├── siniestro-form/    → Modal crear/editar
│       └── siniestro-detail/  → Modal de detalle (solo lectura)
├── layout/
│   ├── navbar/      → Barra superior fija
│   └── sidebar/     → Menú lateral
└── shared/
    └── status-badge/ → Badge reutilizable de estado
```

## Endpoints API

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | /api/siniestro | Lista con filtros opcionales |
| GET    | /api/siniestro/{id} | Detalle por ID |
| POST   | /api/siniestro | Crear nuevo |
| PUT    | /api/siniestro/{id} | Actualizar |
| DELETE | /api/siniestro/{id} | Eliminar |
| GET    | /api/siniestro/comunidades | Lista de comunidades únicas |
| GET    | /api/siniestro/companias | Lista de compañías únicas |

### Filtros disponibles (query params en GET /api/siniestro):
- `?search=texto` — búsqueda global
- `?estado=Abierto|EnProceso|Finalizado`
- `?comunidad=Rioja+17`
- `?companiaSeguros=Ocaso`
- `?fechaDesde=2026-01-01&fechaHasta=2026-12-31`
