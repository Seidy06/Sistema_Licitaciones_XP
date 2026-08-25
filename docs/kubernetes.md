# Kubernetes en el estado actual

Desde HU-33 el repositorio versiona los manifiestos de despliegue de la
aplicación en la carpeta `/k8s`. Aún no se requiere clúster para desarrollar:
las pruebas unitarias validan el contrato declarativo de estos archivos.

## Manifiestos incluidos

| Archivo | Recurso | Contenido |
| --- | --- | --- |
| `k8s/deployment.yaml` | `Deployment` | API `licitaciones-api` (imagen `licitaciones-api:latest`, puerto 8080) con `startupProbe`, `readinessProbe` y `livenessProbe` sobre `/health`, `resources.requests/limits`, conexión a base de datos vía `secretKeyRef` (`licitaciones-secret`) y `Database__ApplyMigrationsOnStartup` vía `configMapKeyRef`. |
| `k8s/service.yaml` | `Service` | ClusterIP que expone el puerto 8080 de la aplicación dentro del clúster. |
| `k8s/configmap.yaml` | `ConfigMap` | Configuración no sensible; mantiene `Database__ApplyMigrationsOnStartup: "false"`: las réplicas de la API nunca aplican migraciones al arrancar. |
| `k8s/secret.yaml` | `Secret` | Plantilla con marcadores `REEMPLAZAR_*`: cadena de conexión y variables `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`. Este repositorio nunca versiona credenciales reales. |
| `k8s/postgres-statefulset.yaml` | `Service` + `StatefulSet` | Servicio headless `postgres` (identidad estable) y PostgreSQL 16 con `volumeClaimTemplates` (`postgres-data`, `ReadWriteOnce`, 1Gi) montado en `/var/lib/postgresql/data`; los datos se conservan entre reinicios del pod gracias al PersistentVolumeClaim. |
| `k8s/migrations-job.yaml` | `Job` | Migraciones controladas: corre el bundle EF una sola vez (`licitaciones-migraciones`), no automáticamente en cada arranque de réplica. |

## Aplicar los manifiestos en un clúster local

Se requiere un clúster local (por ejemplo kind o minikube), `kubectl` y Docker.
Las imágenes deben existir primero en el nodo:

```powershell
docker build -t licitaciones-api .
docker build -f Dockerfile.migrations -t licitaciones-migrations .
# con kind:
kind load docker-image licitaciones-api
kind load docker-image licitaciones-migrations
```

Antes de aplicar, edite `k8s/secret.yaml` y reemplace cada marcador
`REEMPLAZAR_*` por los valores reales; para este clúster use
`Host=postgres` (el Service headless del StatefulSet) y los mismos valores
de base/usuario/contraseña en las cuatro claves. No versione esos valores.

```powershell
kubectl apply -f k8s/
```

El orden dentro de la carpeta es indiferente para `kubectl apply`; el Job de
migraciones puede lanzarse antes o después del Deployment porque se ejecuta
de forma independiente.

## Evidencia esperada en el clúster

```powershell
kubectl get statefulset
kubectl get pods
kubectl get svc
kubectl get pvc
kubectl logs job/licitaciones-migraciones
```

- `kubectl get pods` muestra `postgres-0` en `Running` y el pod del Job
  `licitaciones-migraciones` en `Completed`.
- `kubectl get svc` lista `postgres` (ClusterIP None) y `licitaciones-api`.
- `kubectl get pvc` lista `postgres-data-postgres-0` en `Bound`.

### Conservación de datos tras un reinicio

1. Cree un dato y anote la cantidad de filas (por ejemplo vía la API o
   `psql` dentro del pod).
2. Reinicie el pod: `kubectl delete pod postgres-0` (el StatefulSet lo vuelve
   a crear).
3. Espere a que `kubectl get pods` muestre `postgres-0` en `Running` y
   consulte de nuevo: la fila sigue ahí.

Los datos persisten porque viven en el volumen respaldado por el
PersistentVolumeClaim `postgres-data`, que sobrevive al reinicio del pod;
esta conservación es justamente el criterio de HU-34 y debe evidenciarse con
la secuencia anterior antes de dar la historia por terminada.
