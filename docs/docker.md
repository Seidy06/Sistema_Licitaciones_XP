# Docker en el estado actual

Docker Compose contiene únicamente PostgreSQL 16. No hay Dockerfile ni servicio para Web o API; ambas aplicaciones se ejecutan con `dotnet run` en el equipo local. El volumen `licitaciones_postgres_data` conserva los datos.

## Preparación

Se requiere Docker con Compose y .NET SDK 9. Copie `.env.example` a `.env` y ajuste la contraseña si corresponde:

```powershell
Copy-Item .env.example .env
docker compose config
```

El `.env.example` define base, usuario, contraseña, puerto y `ConnectionStrings__Licitaciones`. Compose consume las cuatro variables de PostgreSQL; la cadena de conexión se utiliza al ejecutar Web o API desde la misma terminal.

## Iniciar y comprobar PostgreSQL

```powershell
docker compose up -d postgres
docker compose ps
docker compose logs postgres
```

El servicio se llama `postgres`, publica `${POSTGRES_PORT:-5432}:5432` y usa `pg_isready` como comprobación de salud.

## Ejecutar la aplicación

Cargue la cadena del `.env` en la sesión o defínala explícitamente:

```powershell
$env:ConnectionStrings__Licitaciones = "Host=localhost;Port=5432;Database=licitaciones_db;Username=licitaciones_user;Password=change_this_password"
dotnet run --project src/Licitaciones.Web
```

Web ejecuta las migraciones pendientes durante el arranque. Para la API, la base debe estar migrada previamente, por ejemplo iniciando Web una vez:

```powershell
dotnet run --project src/Licitaciones.Api
```

## Detener

```powershell
docker compose stop
```

`docker compose down` elimina el contenedor y la red, pero conserva el volumen nombrado si no se agrega `--volumes`. Este documento no recomienda borrar el volumen porque contiene los datos locales.
