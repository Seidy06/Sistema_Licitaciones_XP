# Uso de inteligencia artificial

## Alcance declarado en la Iteración 1

Se utilizó IA como apoyo de programación en pareja y documentación para revisar la estructura de la solución, contrastar los flujos de proveedores entre capas, proponer casos de prueba, ejecutar verificaciones y mantener los documentos técnicos alineados con el repositorio.

## Módulos asistidos

- Proveedores en Domain y Application: normalización, validación, duplicidad, consulta, edición, concurrencia y baja lógica.
- Persistencia: mapeos de EF Core, migraciones, índice único parcial y filtro de bajas.
- Entradas HTTP: API REST y MVC de proveedores.
- Pruebas: revisión de escenarios unitarios e integrados con PostgreSQL real.
- Documentación: arquitectura, datos, API, pruebas, Docker y bitácora XP.
- Licitaciones y ofertas (Iteración 2): análisis de código de dominio (`Licitacion.Publicar`, `LicitacionTransicion`) para la fase refactor de HU-11; refactors de HU-12 y HU-13; y refactor de HU-14 para sustituir la comparación textual de errores por `OfertaDuplicadaException`, acotar la traducción de PostgreSQL al índice esperado y centralizar el mapeo `OfertaDto`. En HU-15, Codex apoyó la creación y ejecución de las pruebas ROJO, el diagnóstico del comportamiento esperado, la revisión del VERDE aportado por la pareja y el refactor de nombres y construcción duplicada de excepciones. En HU-16 ayudó a inspeccionar la cobertura previa, crear las pruebas Application/HTTP, verificar el VERDE con PostgreSQL real y refactorizar la calculadora pura y las aserciones sin cambiar comportamiento. En HU-17 apoyó la inspección de cobertura, las pruebas HTTP, la verificación del VERDE, la eliminación de la proyección duplicada de ofertas y el diagnóstico de Testcontainers que llevó a compartir una sola fixture PostgreSQL entre las pruebas integradas. La verificación final alcanzó 175 pruebas verdes y la documentación se contrastó con código, CI y commits reales. En el cierre documental del 20 de agosto de 2026 la IA apoyó, sin modificar código: verificar con `git log main --merges` que los PR #19 a #26 fusionaron las ocho historias; consultar la API pública de GitHub para confirmar el resultado de CI en cada commit de fusión; evaluar la Definition of Done por historia (excluyendo HU-12 de la velocidad observada por carecer de endpoints HTTP y registro DI); calcular la velocidad observada (26 SP); y realinear `docs/modulos/ofertas.md` con lo implementado por HU-17. Cada dato se contrastó con commits, ejecuciones de CI, controladores y archivos reales antes de registrarse.
- Regularización retrospectiva de trazabilidad de Iteración 1: Codex auditó HU-00 a HU-09, la renumeración histórica de proveedores, los PR #2 a #17 aplicables, commits, pruebas y ejecuciones CI. Creó los Issues retrospectivos #37 a #45 para las historias faltantes y conservó #8 como referencia histórica equivalente a HU-06, añadiendo solo comentarios aclaratorios. La intervención ocurrió después del cierre y de `v0.1.0`, sin modificar código, pruebas, commits, PR, el Issue histórico ni etiquetas.
- Regularización retrospectiva de trazabilidad de Iteración 2: Codex auditó la ausencia de Issues para HU-10 a HU-17, contrastó historias, prioridades y criterios con `historias-usuario.md`, verificó ramas, commits, pruebas, PR #19 a #26, correcciones del PR #28 y ejecuciones reales de GitHub Actions. Después del cierre y de la creación de `v0.2.0`, creó los Issues retrospectivos #29 a #36, añadió comentarios explícitos a los PR y actualizó únicamente documentación. La intervención no modificó código, pruebas, commits históricos, ramas remotas ni etiquetas.

## Ejemplos de apoyo y validación humana

| Apoyo de IA | Validación conservada por el equipo |
| --- | --- |
| Comparar contratos HTTP con controladores. | Revisión directa de rutas, DTO, estados y pruebas. |
| Detectar documentación desactualizada. | Contraste con migraciones, configuración y `git log`. |
| Sugerir comandos reproducibles. | Ejecución local de build y pruebas. |
| Ayudar a describir ciclos TDD. | Confirmación con la secuencia de commits rojo, verde y refactorización. |

La IA no define por sí sola historias, estimaciones, aceptación ni autoría. Las decisiones y la integración permanecen bajo responsabilidad de Seidy y Tiffany. No se deben enviar credenciales, secretos ni datos personales a herramientas externas, y ningún resultado se acepta sin contrastarlo con código, pruebas o historial verificable.
