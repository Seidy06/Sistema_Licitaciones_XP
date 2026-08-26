📋 Sistema de Gestión de Licitaciones

Aplicación web para gestionar licitaciones, proveedores y ofertas económicas. Permite crear licitaciones, recibir ofertas, calcular automáticamente la mejor oferta y su nivel de aprobación, y ver los montos en colones (CRC) o dólares (USD).

Desarrollado con la metodología ágil Extreme Programming (XP).

🛠️ Tecnologías
Backend: .NET 9, ASP.NET Core MVC, API REST
Base de datos: PostgreSQL + Entity Framework Core
Frontend: HTML, CSS, JavaScript, Bootstrap
Pruebas: xUnit, Playwright
Infraestructura: Docker, Kubernetes
CI/CD: GitHub Actions

🚀 Cómo ejecutarlo
bash
git clone https://github.com/Seidy06/Sistema_Licitaciones_XP.git
cd Sistema_Licitaciones_XP
docker compose up --build

📚 Documentación

Toda la documentación está en la carpeta /docs.

- [Visión y alcance](vision-alcance.md)
- [Uso de IA](uso-ia.md)
- [Pruebas](pruebas.md)
- [Plan XP](plan-xp.md)
- [Modelo de datos](modelo-datos.md)
- [Kubernetes](kubernetes.md)
- [Integración de módulos](integracion-modulos.md)
- [HU-08 Borrado lógico proveedores](hu-08-borrado-logico-proveedores.md)
- [HU-09 Listar consultar proveedores](hu-09-listar-consultar-proveedores.md)
- [Historias de usuario](historias-usuario.md)
- [Docker](docker.md)
- [Bitácora XP](bitacora-xp.md)
- [Arquitectura general](arquitectura-general.md)
- [API](api.md)
- [Módulo API REST](modulos/api-rest.md)
- [Módulo interfaz web](modulos/interfaz-web.md)
- [Módulo licitaciones](modulos/licitaciones.md)
- [Módulo niveles de aprobación](modulos/niveles-aprobacion.md)
- [Módulo ofertas](modulos/ofertas.md)
- [Módulo persistencia](modulos/persistencia.md)
- [Módulo proveedores](modulos/proveedores.md)
- [Módulo tipo de cambio](modulos/tipo-cambio.md)
