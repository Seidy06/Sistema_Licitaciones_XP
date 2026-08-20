# Uso de inteligencia artificial

## Alcance declarado en la Iteración 1

Se utilizó IA como apoyo de programación en pareja y documentación para revisar la estructura de la solución, contrastar los flujos de proveedores entre capas, proponer casos de prueba, ejecutar verificaciones y mantener los documentos técnicos alineados con el repositorio.

## Módulos asistidos

- Proveedores en Domain y Application: normalización, validación, duplicidad, consulta, edición, concurrencia y baja lógica.
- Persistencia: mapeos de EF Core, migraciones, índice único parcial y filtro de bajas.
- Entradas HTTP: API REST y MVC de proveedores.
- Pruebas: revisión de escenarios unitarios e integrados con PostgreSQL real.
- Documentación: arquitectura, datos, API, pruebas, Docker y bitácora XP.
- Licitaciones y ofertas (Iteración 2): análisis de código de dominio (`Licitacion.Publicar`, `LicitacionTransicion`) para la fase refactor de HU-11; refactors de HU-12 y HU-13; y refactor de HU-14 para sustituir la comparación textual de errores por `OfertaDuplicadaException`, acotar la traducción de PostgreSQL al índice esperado y centralizar el mapeo `OfertaDto`. Codex también ayudó a ejecutar la línea base y la suite final con PostgreSQL mediante Testcontainers, y a contrastar la documentación con código, pruebas y commits reales.

## Ejemplos de apoyo y validación humana

| Apoyo de IA | Validación conservada por el equipo |
| --- | --- |
| Comparar contratos HTTP con controladores. | Revisión directa de rutas, DTO, estados y pruebas. |
| Detectar documentación desactualizada. | Contraste con migraciones, configuración y `git log`. |
| Sugerir comandos reproducibles. | Ejecución local de build y pruebas. |
| Ayudar a describir ciclos TDD. | Confirmación con la secuencia de commits rojo, verde y refactorización. |

La IA no define por sí sola historias, estimaciones, aceptación ni autoría. Las decisiones y la integración permanecen bajo responsabilidad de Seidy y Tiffany. No se deben enviar credenciales, secretos ni datos personales a herramientas externas, y ningún resultado se acepta sin contrastarlo con código, pruebas o historial verificable.
