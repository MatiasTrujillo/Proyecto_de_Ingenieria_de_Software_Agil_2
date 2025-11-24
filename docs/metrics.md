# Métricas - Definición e implementación

Este documento describe las métricas a instrumentar, cómo medirlas y ejemplos prácticos para integrarlas en el proyecto (.NET WebApi). Incluye queries PromQL y ejemplos de reglas de alerta.

## Resumen 
- Negocio
  - `sales_total` (counter) — Ventas totales. Consulta mensual: `increase(sales_total[30d])`.
  - `reservations_total` (counter) — Reservas totales. Consulta mensual: `increase(reservations_total[30d])`.
- Aplicación
  - `reservation_request_duration_seconds` (histogram) — latencia de POST /api/reservas. Etiquetas: `endpoint`, `method`.
  - `reservation_requests_total` (counter) — peticiones al endpoint. Etiquetas: `endpoint`, `method`, `status_code`.
  - `reservation_errors_total` (counter) — errores (4xx/5xx). Etiquetas: `endpoint`, `method`, `status_code`, `error_type`.
  - Acotación de tasa de errores: medir por endpoint específico (ej. `/api/reservas`) y distinguir 4xx/5xx.
- Infraestructura
  - `container_cpu_usage_seconds_total` (rate -> % CPU) — ventanas de moving average: 5m, 30m, 1h.
  - `container_memory_usage_bytes` — ventanas de moving average: 5m, 30m, 1h.

## Ventanas recomendadas
- Application: métricas de latencia y error: 5m y 30m (usar `rate(...[5m])` o `increase(...[5m])`).
- Infra: promedios móviles 5m, 30m, 1h (usar `avg_over_time(...[30m])`, o `rate(...[5m])` para counters).

## PromQL útiles
- p95 latencia (5m):
```
histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint="/api/reservas"}[5m])))
```
- tasa de errores (5m) para `/api/reservas` (4xx+5xx):
```
(increase(reservation_errors_total{endpoint="/api/reservas"}[5m]) / increase(reservation_requests_total{endpoint="/api/reservas"}[5m])) * 100
```
- CPU % (5m avg) para contenedor `pharmago-webapi`:
```
avg by (container) (rate(container_cpu_usage_seconds_total{container="pharmago-webapi"}[5m])) * 100
```
- Memoria promedio (30m):
```
avg_over_time(container_memory_usage_bytes{container="pharmago-webapi"}[30m])
```

## Reglas de alerta (ejemplos)
- Alta tasa de errores (5% en 5m):
```
alert: HighReservationErrorRate
expr: increase(reservation_errors_total{endpoint="/api/reservas"}[5m]) / increase(reservation_requests_total{endpoint="/api/reservas"}[5m]) > 0.05
for: 10m
labels:
  severity: warning
annotations:
  summary: "High reservation error rate for /api/reservas"
  description: "Error rate > 5% during last 5 minutes"
```
# Guía de implementación y consultas

Este documento describe las métricas implementadas en el proyecto PharmaGo y cómo consultarlas. Incluye:
- Definiciones de cada métrica (negocio, aplicación, infraestructura)
- Queries PromQL para consultar en Prometheus/Grafana
- Scripts de prueba para generar tráfico de test
- Ejemplos de resultados obtenidos

## Contexto del feedback recibido
Según las recomendaciones de los profesores:
- Tasa de errores: se definió específicamente qué endpoints y códigos HTTP (4xx, 5xx o ambos) se cuentan
- Métricas de infraestructura: se especificaron ventanas móviles (5m, 30m, 1h) ya que las time series requieren una ventana para calcular promedios

## 1) Métricas implementadas

### Métricas de negocio
- **`sales_total`** (counter): contador de ventas registradas
  - Consulta mensual: `increase(sales_total[30d])`
- **`reservations_total`** (counter): contador de reservas realizadas
  - Consulta mensual: `increase(reservations_total[30d])`

### Métricas de aplicación
- **`reservation_requests_total`** (counter): total de solicitudes al endpoint de reservas
  - Labels: `endpoint`, `method`, `status_code`
- **`reservation_errors_total`** (counter): solicitudes que terminaron en error
  - Labels: `endpoint`, `method`, `status_code`, `error_type`
- **`reservation_request_duration_seconds`** (histogram): latencia de solicitudes de reserva
  - Labels: `endpoint`, `method`
  - Buckets configurados: [0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10]

### Métricas de infraestructura (cAdvisor)
- **`container_cpu_usage_seconds_total`** (counter): tiempo de CPU consumido por contenedor
- **`container_memory_usage_bytes`** (gauge): memoria utilizada por contenedor

## 2) Configuración implementada

### Definición de tasa de errores
Se implementaron dos opciones configurables mediante variables en Grafana:
- **Opción A** (implementada por defecto): cuenta errores 4xx + 5xx (todas las respuestas no exitosas)
- **Opción B**: cuenta únicamente 5xx (errores del servidor)

La variable `$status` permite filtrar con expresiones regulares: `^4..$`, `^5..$`, o `^(4..|5..)$`

### Ventanas temporales
Se configuraron las siguientes ventanas para moving averages:
- **5m**: para métricas de aplicación (latencia y tasa de errores)
- **30m**: para promedios de infraestructura
- **1h**: para análisis de tendencias más amplias

La variable `$window` en Grafana permite cambiar dinámicamente entre estas opciones.

## 3) Consultas PromQL utilizadas

Las siguientes queries se utilizan en nuestro dashboard de Grafana. En producción se reemplaza el endpoint hardcodeado por la variable `$endpoint`.

### Ventana mensual (conteo de negocio)
- Ventas mensuales:
```
increase(sales_total[30d])
```
- Reservas mensuales (si `reservations_total` existe):
```
increase(reservations_total[30d])
```
- Alternativa (si no hay `reservations_total`): sumar `reservation_requests_total`:
```
increase(reservation_requests_total{endpoint=~"$endpoint"}[30d])
```

### p95 latencia para reservas (usa `$window`)
```
histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint=~"$endpoint"}[$window])))
```

### Tasa de errores (%) (usar `$window` y `$status` para seleccionar 4xx/5xx)
```
(increase(reservation_errors_total{endpoint=~"$endpoint",status_code=~"$status"}[$window])
 / increase(reservation_requests_total{endpoint=~"$endpoint"}[$window])) * 100
```

### Tasa de errores absoluta (conteo) en `$window`
```
increase(reservation_errors_total{endpoint=~"$endpoint",status_code=~"$status"}[$window])
```

### CPU % del contenedor (dos variantes)
 - Si existe `container_spec_cpu_quota` y `container_spec_cpu_period` (cAdvisor):
```
# cores asignados por contenedor (no siempre expuesto como métrica directa)
# CPU%:
100 * rate(container_cpu_usage_seconds_total{container=~"$container"}[$window])
 / (container_spec_cpu_quota{container=~"$container"} / container_spec_cpu_period{container=~"$container"})
```

 - Fallback (si no hay cuota clara, aproximación por núcleo):
```
100 * rate(container_cpu_usage_seconds_total{container=~"$container"}[$window])
```

### Memoria promedio del contenedor (ejemplo, en MiB)
```
avg_over_time(container_memory_usage_bytes{container=~"$container"}[$window]) / 1024 / 1024
```

## 4) Reglas de alerta configuradas

### Alta tasa de errores
```
alert: HighReservationErrorRate
expr: (increase(reservation_errors_total{endpoint=~"$endpoint"}[5m]) / increase(reservation_requests_total{endpoint=~"$endpoint"}[5m])) > 0.05
for: 10m
labels:
  severity: warning
```

### Latencia p95 elevada
```
alert: HighReservationLatency
expr: histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint=~"$endpoint"}[5m]))) > 1.5
for: 5m
labels:
  severity: warning
```

## 5) Variables configuradas en Grafana

Las siguientes variables se configuraron en el dashboard para permitir filtros dinámicos:

- **`$DS_PROMETHEUS`**: datasource de Prometheus
- **`$window`**: ventana temporal para `rate()`, `increase()` y `avg_over_time()`
  - Valores disponibles: `5m`, `30m`, `1h`
- **`$endpoint`**: filtro de endpoint (ej. `/api/reserva`)
- **`$status`**: filtro de códigos HTTP
  - Valores: `^2..$` (2xx), `^4..$` (4xx), `^5..$` (5xx), `^(4..|5..)$` (errores)
- **`$container`**: selector de contenedor Docker

## 6) Scripts de prueba

Para validar las métricas, se utilizaron los siguientes scripts PowerShell que generan tráfico de prueba.

### Configuración inicial

```powershell
$API = 'http://localhost:5000'
$EP_RESERVA = '/api/reserva'
```

### Caso 1: Reservas exitosas (201)

```powershell
$body = @{ NombreMedicamento = 'PARACETAMOL'; Cantidad = 1; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 10; $i++) {
  Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($body | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue
  Start-Sleep -Milliseconds 200
}
```

### Caso 2: Bad requests (400)

```powershell
for ($i=0; $i -lt 3; $i++) {
  # cuerpo inválido (falta campo obligatorio)
  $bad = '{ "NombreMedicamento": 123 '
  try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body $bad -ContentType 'application/json' } catch { }
  Start-Sleep -Milliseconds 200
}
```

### Caso 3: Errores del servidor (500)

```powershell
$bodyFail = @{ NombreMedicamento = 'FORCE500'; Cantidad = 1; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 2; $i++) {
  try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($bodyFail | ConvertTo-Json) -ContentType 'application/json' } catch { }
  Start-Sleep -Milliseconds 200
}
```

### Caso 4: Ventas

```powershell
# Ajustar $EP_VENTA al endpoint correcto
$EP_VENTA = '/api/venta'
$sale = @{ ProductoId = 123; Cantidad = 2; Precio = 250.0; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 5; $i++) {
  Invoke-RestMethod -Method Post -Uri "$API$EP_VENTA" -Body ($sale | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue
  Start-Sleep -Milliseconds 300
}
```

### Script completo (todos los casos)

```powershell
# ejecutar todo en secuencia
# 10 éxitos
for ($i=0;$i -lt 10;$i++) { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($body | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue; Start-Sleep -Milliseconds 150 }
# 3 bad
for ($i=0;$i -lt 3;$i++) { try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body '{ "bad": }' -ContentType 'application/json' } catch { }; Start-Sleep -Milliseconds 150 }
# 2 500
for ($i=0;$i -lt 2;$i++) { try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($bodyFail | ConvertTo-Json) -ContentType 'application/json' } catch { }; Start-Sleep -Milliseconds 150 }
```

### Resultado esperado
Después de ejecutar estos scripts, Prometheus registra:
- Incrementos en `reservation_requests_total` con los códigos 201/400/500
- Incrementos en `reservation_errors_total` para requests fallidos
- Actualizaciones en los histogram buckets de `reservation_request_duration_seconds`

## 7) Cómo verificar las métricas en Prometheus

### Queries de validación
Estas consultas permiten verificar que las métricas se están recolectando correctamente:

**Validar requests totales:**
```promql
reservation_requests_total{endpoint=~"$endpoint"}
```

**Verificar errores por código HTTP:**
```promql
sum by (status_code) (increase(reservation_errors_total{endpoint=~"$endpoint"}[$window]))
```

**Consultar p95 de latencia:**
```promql
histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint=~"$endpoint"}[$window])))
```

---

## 8) Ejemplos de resultados obtenidos

### Nota sobre las capturas
Al ejecutar los tests con tráfico simultáneo, las gráficas pueden mostrar patrones temporales similares ya que todas responden al mismo evento. Sin embargo, cada métrica mide una dimensión diferente del sistema:
- **Negocio**: contadores acumulados (unidad: cantidad)
- **Aplicación**: latencias y tasas (unidades: segundos, porcentaje)
- **Infraestructura**: recursos del sistema (unidades: porcentaje CPU, bytes)

### Datos de la prueba realizada
- **Fecha/hora**: 2025-11-14 16:35 UTC-3
- **Ventana configurada**: `$window = 5m`
- **Tráfico generado**: 10 reservas exitosas, 3 bad requests (400), 2 errores server (500)

### Ejemplo 1: Métrica de negocio (reservas mensuales)
**Query PromQL**:
```promql
increase(reservation_requests_total{endpoint="/api/reserva"}[30d])
```

**Resultado de ejemplo** (instant query):
```json
{
  "status": "success",
  "data": {
    "resultType": "vector",
    "result": [
      {
        "metric": {
          "endpoint": "/api/reserva",
          "method": "POST"
        },
        "value": [1731604524, "156"]
      }
    ]
  }
}
```
**Interpretación**: 156 reservas acumuladas en los últimos 30 días para el endpoint `/api/reserva`.

### Ejemplo 2: Métrica de aplicación (p95 latencia)
**Query PromQL**:
```promql
histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint="/api/reserva"}[5m])))
```

**Resultado de ejemplo**:
```json
{
  "status": "success",
  "data": {
    "resultType": "vector",
    "result": [
      {
        "metric": {},
        "value": [1731604524, "0.085"]
      }
    ]
  }
}
```
**Interpretación**: el percentil 95 de latencia en la ventana de 5m fue 85ms (0.085 segundos).

### Ejemplo 3: Métrica de aplicación (tasa de errores %)
**Query PromQL**:
```promql
(increase(reservation_errors_total{endpoint="/api/reserva",status_code=~"^(4..|5..)$"}[5m]) 
 / increase(reservation_requests_total{endpoint="/api/reserva"}[5m])) * 100
```

**Resultado de ejemplo**:
```json
{
  "status": "success",
  "data": {
    "resultType": "vector",
    "result": [
      {
        "metric": {},
        "value": [1731604524, "33.33"]
      }
    ]
  }
}
```
**Interpretación**: 33.33% de las solicitudes en la ventana de 5m terminaron en error (4xx o 5xx).

### Ejemplo 4: Métrica de infraestructura (CPU %)
**Query PromQL**:
```promql
100 * rate(container_cpu_usage_seconds_total{id=~"/docker.*"}[5m])
```

**Resultado de ejemplo** (primeras series):
```json
{
  "status": "success",
  "data": {
    "resultType": "vector",
    "result": [
      {
        "metric": {
          "id": "/docker",
          "name": "pharmago-api"
        },
        "value": [1731604524, "2.45"]
      },
      {
        "metric": {
          "id": "/docker",
          "name": "prometheus"
        },
        "value": [1731604524, "0.87"]
      }
    ]
  }
}
```
**Interpretación**: el contenedor `pharmago-api` está usando 2.45% CPU promedio en la ventana de 5m; `prometheus` usa 0.87%.

### Ejemplo 5: Métrica de infraestructura (Memoria en MiB)
**Query PromQL**:
```promql
avg_over_time(container_memory_usage_bytes{id=~"/docker.*"}[5m]) / 1024 / 1024
```

**Resultado de ejemplo**:
```json
{
  "status": "success",
  "data": {
    "resultType": "vector",
    "result": [
      {
        "metric": {
          "id": "/docker",
          "name": "pharmago-api"
        },
        "value": [1731604524, "345.2"]
      },
      {
        "metric": {
          "id": "/docker",
          "name": "pharmago-db"
        },
        "value": [1731604524, "512.8"]
      }
    ]
  }
}
```
**Interpretación**: contenedor `pharmago-api` usa ~345 MiB de memoria; `pharmago-db` usa ~513 MiB.

### Material de evidencia incluido
Para documentar la implementación se incluyen:
1. **Este archivo** (`docs/metrics.md`): definiciones, queries y scripts de prueba
2. **Captura de dashboard**: mostrando las tres capas (negocio, aplicación, infraestructura)
3. **Nota explicativa**: las gráficas reflejan el mismo evento de tráfico pero miden aspectos diferentes del sistema

Este conjunto de documentación permite reproducir completamente las pruebas utilizando las queries PromQL y scripts PowerShell detallados en este documento.

