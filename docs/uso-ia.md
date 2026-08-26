# Uso de Inteligencia Artificial

## Alcance General

La IA se utilizó como apoyo en programación en pareja y documentación a lo largo de las 4 iteraciones del proyecto. La herramienta empleada fue **OpenAI Codex**, siempre bajo supervisión y validación humana.

## Resumen por Iteración

### Iteración 1
- **Módulos asistidos**: Proveedores (Domain/Application), Persistencia (EF Core, migraciones), API REST/MVC, Pruebas unitarias e integradas, Documentación (arquitectura, datos, API, pruebas, Docker, bitácora XP).
- **Enfoque**: Verificación de flujos, contraste de capas, propuesta y ejecución de casos de prueba, mantenimiento de documentación alineada con el repositorio.

### Iteración 2
- **Historias asistidas (HU-11 a HU-17)**:
  - Refactor de lógica de dominio, excepciones y mapeos de DTOs.
  - Creación y verificación de pruebas ROJO/VERDE.
  - Inspección de cobertura, pruebas HTTP con PostgreSQL real.
  - Diagnóstico de Testcontainers y consolidación de fixtures.
- **Cierre**: Verificación de fusiones (PR #19-#26), evaluación de Definition of Done y cálculo de velocidad (26 SP).

### Iteración 3
- **Historias asistidas (HU-18 a HU-27)**:
  - Refactor de servicios, centralización de lógica y eliminación de duplicación.
  - Verificación de suites completas (hasta 233 pruebas verdes).
  - Diagnóstico de incidencias (mojibake, escapa de HTML, íconos estáticos).
  - Documentación de módulos (`niveles-aprobacion`, `tipo-cambio`, `interfaz-web`, `api-rest`).
- **Cierre**: Consolidación de ciclos TDD, rotación Driver/Navigator, velocidad 38 SP y registro de observaciones para siguiente iteración.

### Iteración 4
- **Historias asistidas (HU-28 a HU-37)**:
  - Creación de pruebas unitarias de dominio/application para aumentar cobertura (hasta 314 pruebas verdes).
  - Refactor de pruebas funcionales, E2E con Playwright y Testcontainers.
  - Contratos de infraestructura (Dockerfile, compose.yaml, manifiestos Kubernetes, pipeline CI).
  - Verificación de documentación general (README, diagramas, bitácora, enlaces).
  - Creación de tag local `v1.0.0` para entrega final.
- **Cierre**: Verificación de todos los PR fusionados, cumplimiento de Definition of Done y velocidad 45 SP (con riesgo por desviación de 36 SP planificados).

## Ejemplos de Apoyo y Validación Humana

| Apoyo de IA | Validación Humana |
|-------------|-------------------|
| Comparar contratos HTTP con controladores | Revisión de rutas, DTOs, estados y pruebas |
| Detectar documentación desactualizada | Contraste con migraciones y `git log` |
| Sugerir comandos reproducibles | Ejecución local de build y pruebas |
| Ayudar a describir ciclos TDD | Confirmación con commits rojo, verde y refactor |

## Notas Finales

- Todas las intervenciones de IA fueron revisadas y validadas por el equipo.
- No se modificó código sin supervisión humana.
- La trazabilidad se mantuvo mediante Issues, PRs y commits documentados.