# Visión y alcance

## Visión

Construir una aplicación web modular, mantenible, verificable y desplegable que
centralice la gestión de licitaciones, proveedores, ofertas económicas, niveles
de aprobación y conversión referencial de moneda. El producto se desarrollará
de manera incremental, aplicando exclusivamente Extreme Programming (XP), con
entregas pequeñas y frecuentes, pruebas automatizadas e integración continua.

## Objetivo del producto

Proporcionar una solución que permita administrar el ciclo de una licitación,
desde su creación y publicación hasta el registro y comparación de ofertas,
aplicando reglas de negocio e integridad de datos en la interfaz, el servidor y
PostgreSQL. Las operaciones principales estarán disponibles mediante ASP.NET
Core MVC y una API REST documentada.

## Problema que resuelve

La gestión manual o dispersa de licitaciones dificulta controlar proveedores,
fechas de cierre, presupuestos, ofertas, aprobaciones y conversiones monetarias
de manera consistente. Esto puede provocar registros duplicados, aceptación de
ofertas vencidas o superiores al presupuesto, cálculos no uniformes y falta de
trazabilidad.

El sistema concentra esa información, automatiza las validaciones, conserva los
montos oficiales en colones costarricenses (CRC) y permite identificar la mejor
oferta, su clasificación de ahorro y el nivel de aprobación correspondiente.

## Usuarios principales

- **Encargado de licitaciones:** administra proveedores, licitaciones y ofertas;
  publica o cierra licitaciones y consulta la mejor oferta.
- **Administrador del sistema:** configura niveles de aprobación y tipos de
  cambio.
- **Aprobador o responsable de decisión:** consulta la mejor oferta, el ahorro
  obtenido y el nivel de aprobación requerido.
- **Sistema cliente o integrador:** consume las operaciones mediante la API
  REST.
- **Equipo de desarrollo y despliegue:** prueba, contiene, integra y despliega
  la solución.

## Alcance incluido

- Landing page con explicación del propósito y flujo general del sistema.
- Navegación visible entre los módulos y la documentación interactiva de la API.
- Diseño adaptable para computadoras y dispositivos móviles.
- CRUD completo de proveedores, licitaciones, ofertas, niveles de aprobación y
  tipos de cambio.
- Ciclo de estados de licitación: borrador, publicada y cerrada.
- Validación de fecha y hora de cierre en la zona `America/Costa_Rica`, con
  comparaciones internas en UTC.
- Validación de presupuestos, ofertas duplicadas, ofertas vencidas y ofertas
  superiores al presupuesto.
- Cálculo de mejor oferta en CRC, desempate, porcentaje de ahorro,
  clasificación y nivel de aprobación.
- CRC como moneda oficial y fuente de verdad.
- Visualización referencial en USD mediante un tipo de cambio administrable,
  sin modificar los valores almacenados en CRC.
- Modos claro y oscuro con persistencia de la preferencia.
- Interfaz ASP.NET Core MVC y API REST versionada y documentada.
- Persistencia exclusiva en PostgreSQL, con migraciones, datos semilla,
  restricciones, auditoría y concurrencia optimista.
- Pruebas unitarias, de integración con PostgreSQL real y funcionales de
  extremo a extremo.
- Ejecución reproducible mediante Docker y Docker Compose.
- Despliegue en Kubernetes con persistencia y configuración segura.
- Integración continua mediante GitHub Actions.
- Documentación técnica y metodológica en Markdown dentro de `/docs`.

## Fuera del alcance

- Pagos electrónicos.
- Firma digital.
- Envío de correos.
- Integración con sistemas gubernamentales o fuentes externas de tipo de cambio.
- Aplicación móvil nativa.
- Sustituir PostgreSQL por SQLite en la aplicación o en las pruebas de
  integración.
- Funcionalidades no solicitadas en el documento oficial.

## Restricciones técnicas

- El proyecto debe aplicar exclusivamente Extreme Programming (XP); no se
  utilizarán marcos ágiles alternativos como metodología rectora.
- La solución debe ser modular. Puede implementarse como monolito modular o
  mediante microservicios únicamente si la separación está técnicamente
  justificada.
- Los controladores deben ser delgados y la lógica de negocio debe residir en
  servicios o capas apropiadas.
- Los identificadores se generan automáticamente y no son editables.
- Los valores monetarios deben usar `decimal` con precisión explícita, por
  ejemplo `numeric(18,2)`; no se permite `float` ni `double`.
- Las fechas deben almacenarse con `DateTimeOffset` o una estrategia
  equivalente, compararse internamente en UTC y mostrarse en
  `America/Costa_Rica`.
- La persistencia de la aplicación y las pruebas de integración debe realizarse
  exclusivamente con PostgreSQL.
- Las credenciales y cadenas de conexión deben proporcionarse mediante
  variables de entorno o secretos; no deben almacenarse credenciales reales en
  el repositorio.
- La interfaz debe funcionar sin depender exclusivamente de recursos obtenidos
  desde una CDN.
- La API debe utilizar DTO, versionado, OpenAPI/Swagger, códigos HTTP
  apropiados y respuestas `ProblemDetails`; no debe exponer directamente
  entidades de Entity Framework Core.
- La capa Domain y Application debe alcanzar al menos 80 % de cobertura de
  líneas, y el proyecto completo al menos 70 %.
- Toda la documentación producida por el equipo debe mantenerse en Markdown
  dentro de `/docs`; las imágenes y evidencias deben ubicarse en `/docs/assets`.
- El historial de Git debe demostrar trabajo incremental, pruebas,
  refactorización, integración continua y trazabilidad con las historias.

## Tecnologías obligatorias

- .NET 9.
- ASP.NET Core MVC.
- ASP.NET Core Web API.
- Entity Framework Core 9.
- PostgreSQL 16 o superior y un proveedor compatible para Entity Framework
  Core.
- HTML5, CSS3 y JavaScript.
- Bootstrap o una biblioteca visual equivalente.
- Docker y Docker Compose.
- Kubernetes.
- Git, GitHub y GitHub Actions.
- xUnit, NUnit o MSTest para pruebas automatizadas.
- Playwright o Selenium para pruebas funcionales de navegador.
- Testcontainers o un mecanismo equivalente para pruebas de integración con
  PostgreSQL real.
