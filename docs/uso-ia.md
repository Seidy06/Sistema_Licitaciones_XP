# Uso de inteligencia artificial

## HU-01 - Registrar proveedor

La IA se utilizó como apoyo de programación en pareja para revisar la
implementación MVC, contrastarla con las capas existentes, ejecutar las pruebas
y mantener la documentación técnica consistente.

### Actividades asistidas

- Inspección de Domain, Application, Infrastructure, API y Web.
- Revisión de que el controlador delegara en `CrearProveedorService`.
- Comprobación de mensajes de validación junto al campo.
- Ejecución de build, pruebas unitarias, integración y prueba HTTP del flujo
  MVC con antifalsificación.
- Documentación del caso de uso, modelo de datos, API, integración y ciclo TDD.

### Decisiones humanas conservadas

- La historia, criterios de aceptación, rama, Issue #8 y Pull Request #9 fueron
  definidos por el equipo.
- La rotación confirmada fue Persona B como Driver y Persona A como Navigator
  para la interfaz MVC.
- El equipo conserva la responsabilidad de revisar el código, validar los
  resultados de GitHub Actions y aprobar la integración del Pull Request.

### Controles

- No se enviaron credenciales ni datos personales a servicios externos.
- No se aceptaron resultados sin contrastarlos con el repositorio y las
  pruebas ejecutables.
- La IA no sustituyó las reglas de negocio existentes ni las duplicó en los
  controladores.
