# Docker en el estado actual

Docker Compose contiene únicamente PostgreSQL 16. El volumen `licitaciones_postgres_data` conserva los datos.

Desde HU-31 el repositorio incluye un `Dockerfile` multi-stage para la API: la
etapa `build` compila con `mcr.microsoft.com/dotnet/sdk:9.0` y la etapa final
ejecuta únicamente con `mcr.microsoft.com/dotnet/aspnet:9.0`, corre como
usuario no root (`USER $APP_UID`) e incluye un `HEALTHCHECK` que consulta
`/health` cada 30 segundos. La aplicación MVC (`Licitaciones.Web`) sigue
ejecutándose con `dotnet run` en el equipo local. Compose todavía no orquesta
Web ni API; esa orquestación corresponde a HU-32.

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

## Construir y ejecutar la imagen de la API (HU-31)

El `Dockerfile` publica `Licitaciones.Api` en Release y expone el puerto 8080.
La construcción copia únicamente `src/` gracias al `.dockerignore`:

```powershell
docker build -t licitaciones-api .
docker run --rm -p 8080:8080 --name licitaciones-api `
  -e ConnectionStrings__Licitaciones="Host=host.docker.internal;Port=5432;Database=licitaciones_db;Username=licitaciones_user;Password=change_this_password" `
  licitaciones-api
```

El contenedor arranca `Licitaciones.Api.dll`, escucha en `http://+:8080`,
responde `GET /health` con `Healthy` y se comprueba a sí mismo con
`HEALTHCHECK CMD curl --fail http://localhost:8080/health`. El proceso corre
con el usuario no privilegiado `$APP_UID` que provee la imagen base.

## Detener

```powershell
docker compose stop
```

`docker compose down` elimina el contenedor y la red, pero conserva el volumen nombrado si no se agrega `--volumes`. Este documento no recomienda borrar el volumen porque contiene los datos locales.
