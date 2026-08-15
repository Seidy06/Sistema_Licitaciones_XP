# Uso de inteligencia artificial

## Alcance declarado en la Iteración 1

Se utilizó IA como apoyo de programación en pareja y documentación para revisar la estructura de la solución, contrastar los flujos de proveedores entre capas, proponer casos de prueba, ejecutar verificaciones y mantener los documentos técnicos alineados con el repositorio.

## Módulos asistidos

- Proveedores en Domain y Application: normalización, validación, duplicidad, consulta, edición, concurrencia y baja lógica.
- Persistencia: mapeos de EF Core, migraciones, índice único parcial y filtro de bajas.
- Entradas HTTP: API REST y MVC de proveedores.
- Pruebas: revisión de escenarios unitarios e integrados con PostgreSQL real.
- Documentación: arquitectura, datos, API, pruebas, Docker y bitácora XP.

## Ejemplos de apoyo y validación humana

| Apoyo de IA | Validación conservada por el equipo |
| --- | --- |
| Comparar contratos HTTP con controladores. | Revisión directa de rutas, DTO, estados y pruebas. |
| Detectar documentación desactualizada. | Contraste con migraciones, configuración y `git log`. |
| Sugerir comandos reproducibles. | Ejecución local de build y pruebas. |
| Ayudar a describir ciclos TDD. | Confirmación con la secuencia de commits rojo, verde y refactorización. |

La IA no define por sí sola historias, estimaciones, aceptación ni autoría. Las decisiones y la integración permanecen bajo responsabilidad de Seidy y Tiffany. No se deben enviar credenciales, secretos ni datos personales a herramientas externas, y ningún resultado se acepta sin contrastarlo con código, pruebas o historial verificable.
