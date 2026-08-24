# Kubernetes en el estado actual

Desde HU-33 el repositorio versiona los manifiestos de despliegue de la
aplicación en la carpeta `/k8s`. Aún no se requiere clúster para desarrollar:
las pruebas unitarias validan el contrato declarativo de estos archivos.

## Manifiestos incluidos

| Archivo | Recurso | Contenido |
| --- | --- | --- |
| `k8s/deployment.yaml` | `Deployment` | API `licitaciones-api` (imagen `licitaciones-api:latest`, puerto 8080) con `startupProbe`, `readinessProbe` y `livenessProbe` sobre `/health`, `resources.requests/limits`, conexión a base de datos vía `secretKeyRef` (`licitaciones-secret`) y `Database__ApplyMigrationsOnStartup` vía `configMapKeyRef`. |
| `k8s/service.yaml` | `Service` | ClusterIP que expone el puerto 8080 de la aplicación dentro del clúster. |
| `k8s/configmap.yaml` | `ConfigMap` | Configuración no sensible; deja `Database__ApplyMigrationsOnStartup: "false"` hasta que HU-34 defina la estrategia de migraciones en el clúster. |
| `k8s/secret.yaml` | `Secret` | Plantilla con marcadores `REEMPLAZAR_*`; este repositorio nunca versiona credenciales reales. |

## Aplicar los manifiestos en un clúster local

Se requiere un clúster local (por ejemplo kind o minikube) y `kubectl`. La
imagen debe existir primero en el nodo:

```powershell
docker build -t licitaciones-api .
# con kind: kind load docker-image licitaciones-api
kubectl apply -f k8s/
```

Antes de aplicar, reemplace los marcadores del Secret por los valores reales
de su entorno; no los versione.

El `Service` es de tipo ClusterIP: solo es alcanzable dentro del clúster. La
persistencia de PostgreSQL en Kubernetes y la estrategia de migraciones como
Job corresponden a HU-34 y quedan fuera de este documento hasta existir.
