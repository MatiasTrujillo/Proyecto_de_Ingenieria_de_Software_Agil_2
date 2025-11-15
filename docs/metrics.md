# Métricas - Definición e implementación

Este documento describe las métricas a instrumentar, cómo medirlas y ejemplos prácticos para integrarlas en el proyecto (.NET WebApi). Incluye queries PromQL y ejemplos de reglas de alerta.

## Resumen (cambios solicitados)
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
# Métricas - Definición, casos de prueba y PromQL

Este documento actualiza y clarifica las métricas según el feedback del equipo/profesores. Contiene:
- definiciones precisas de cada métrica (negocio, aplicación, infraestructura),
- recomendaciones de ventanas (moving averages) y variables para el dashboard,
- queries PromQL listas para usar en Grafana/Prometheus,
- ejemplos y scripts para generar tráfico de prueba (201, 4xx, 5xx) en PowerShell.

Notas clave del feedback recibido:
- Acotar la "tasa de errores": definir qué endpoints y qué clases de status (4xx, 5xx o ambos) se cuentan.
- Para métricas infra: definir la ventana móvil que se usará (ej. 5m, 30m, 1h). No existe un "promedio" sin ventana.

## 1) Lista de métricas y definiciones

### Negocio
- `sales_total` (counter): contador acumulativo de ventas registradas. Uso mensual: `increase(sales_total[30d])`.
- `reservations_total` (counter): contador acumulativo de reservas. Uso mensual: `increase(reservations_total[30d])`.

### Aplicación (implementadas o planificadas)
- `reservation_requests_total` (counter): total de solicitudes recibidas al endpoint de reservas. Labels recomendadas: `endpoint`, `method`, `status_code`, `user_id` (opcional).
- `reservation_errors_total` (counter): total de solicitudes que terminaron en error. Labels: `endpoint`, `method`, `status_code`, `error_type`.
- `reservation_request_duration_seconds` (histogram): latencia de la solicitud de reserva (buckets recomendados, p.ej. [0.005,0.01,0.025,0.05,0.1,0.25,0.5,1,2.5,5,10]). Labels: `endpoint`, `method`.

### Infraestructura
- `container_cpu_usage_seconds_total` (counter, expuesto por cAdvisor): resumir/derivar uso de CPU por contenedor.
- `container_memory_usage_bytes` (gauge): uso de memoria por contenedor en bytes.

## 2) Decisiones operativas (definiciones concretas)
- Error rate (recomendación): ofrecemos 2 opciones — elegir una para el dashboard:
  - Opción A (recomendada): contar 4xx + 5xx (todas las respuestas no exitosas).
  - Opción B: contar sólo 5xx si se quiere medir errores server-side exclusivamente.
  En las queries y en el dashboard se propone una variable `$status` que permita seleccionar `4..|5..|^(4..|5..)$` (Grafana regex) y así filtrar.
- Ventanas de moving average (variables recomendadas en Grafana): `$window` con valores típicos `5m`, `30m`, `1h`. Use `$window` en `rate()`/`increase()` y en `avg_over_time()` según el tipo de métrica.

## 3) PromQL recomendadas (plantillas)

Nota: reemplace `{endpoint="/api/reserva"}` por el endpoint real en su servicio (p. ej. `/api/reserva` o `/api/reservas`). En Grafana use la variable `$endpoint`.

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

## 4) Ejemplos de alertas (plantilla)

- Alta tasa de errores (opción contando 4xx+5xx):
```
alert: HighReservationErrorRate
expr: (increase(reservation_errors_total{endpoint=~"$endpoint"}[5m]) / increase(reservation_requests_total{endpoint=~"$endpoint"}[5m])) > 0.05
for: 10m
labels:
  severity: warning
```

- Latencia p95 alta:
```
alert: HighReservationLatency
expr: histogram_quantile(0.95, sum by (le) (rate(reservation_request_duration_seconds_bucket{endpoint=~"$endpoint"}[5m]))) > 1.5
for: 5m
labels:
  severity: warning
```

## 5) Recomendaciones para Grafana (variables)
- `$DS_PROMETHEUS` — datasource Prometheus
- `$window` — rango para `rate()`/`increase()`/`avg_over_time()` (valores propuestos: `5m`, `30m`, `1h`)
- `$endpoint` — regex del endpoint a filtrar (ej. `/api/reserva|/api/reservas`)
- `$status` — regex para clases de status: `^2..$`, `^4..$`, `^5..$`, o combinaciones como `^(4..|5..)$`
- `$container` — nombre del contenedor a observar (p. ej. `pharmago-api`, `grafana`, etc.)

## 6) Casos de prueba y comandos PowerShell (generar tráfico)

Reemplace `$API` por la URL real de su API (p. ej. `http://localhost:5000`). Reemplace `$EP_RESERVA` por el path real del endpoint de reservas (p.ej. `/api/reserva`).

```powershell
$API = 'http://localhost:5000'
$EP_RESERVA = '/api/reserva'
```

1) Generar 10 reservas exitosas (201 esperada)

```powershell
$body = @{ NombreMedicamento = 'PARACETAMOL'; Cantidad = 1; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 10; $i++) {
  Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($body | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue
  Start-Sleep -Milliseconds 200
}
```

2) Generar 3 bad requests (400) — enviar JSON incompleto / malformado

```powershell
for ($i=0; $i -lt 3; $i++) {
  # cuerpo inválido (falta campo obligatorio)
  $bad = '{ "NombreMedicamento": 123 '
  try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body $bad -ContentType 'application/json' } catch { }
  Start-Sleep -Milliseconds 200
}
```

3) Generar 2 errores 500 (si mantiene el shortcut de prueba `FORCE500` en controller)

```powershell
$bodyFail = @{ NombreMedicamento = 'FORCE500'; Cantidad = 1; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 2; $i++) {
  try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($bodyFail | ConvertTo-Json) -ContentType 'application/json' } catch { }
  Start-Sleep -Milliseconds 200
}
```

4) Simular ventas (si su endpoint de venta es `/api/venta` o `/api/purchase`):

```powershell
# Ajustar $EP_VENTA al endpoint correcto
$EP_VENTA = '/api/venta'
$sale = @{ ProductoId = 123; Cantidad = 2; Precio = 250.0; Fecha = (Get-Date).ToString('o') }
for ($i=0; $i -lt 5; $i++) {
  Invoke-RestMethod -Method Post -Uri "$API$EP_VENTA" -Body ($sale | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue
  Start-Sleep -Milliseconds 300
}
```

5) Script rápido todo-en-uno (éxitos, 400s, 500s)

```powershell
# ejecutar todo en secuencia
# 10 éxitos
for ($i=0;$i -lt 10;$i++) { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($body | ConvertTo-Json) -ContentType 'application/json' -ErrorAction SilentlyContinue; Start-Sleep -Milliseconds 150 }
# 3 bad
for ($i=0;$i -lt 3;$i++) { try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body '{ "bad": }' -ContentType 'application/json' } catch { }; Start-Sleep -Milliseconds 150 }
# 2 500
for ($i=0;$i -lt 2;$i++) { try { Invoke-RestMethod -Method Post -Uri "$API$EP_RESERVA" -Body ($bodyFail | ConvertTo-Json) -ContentType 'application/json' } catch { }; Start-Sleep -Milliseconds 150 }
```

Tras ejecutar estos scripts, en Prometheus debería verse incremento en `reservation_requests_total` y `reservation_errors_total` con códigos de estado 201/400/500, y los histogram buckets deberían reflejar la latencia.

## 7) Verificación rápida (Prometheus queries)

- Validar counter de requests: `reservation_requests_total{endpoint=~"$endpoint"}`
- Verificar errores por código: `sum by (status_code) (increase(reservation_errors_total{endpoint=~"$endpoint"}[$window]))`
- Ver p95: usar la query de p95 anterior y ajustar `$window`.

## 8) Siguientes pasos sugeridos
- Decidid si la tasa de errores debe contar 4xx+5xx o sólo 5xx y confirmad `$endpoint` real. Yo puedo luego:
  1) Instrumentar `sales_total` en el flujo de ventas y desplegar la imagen (opción: hago el cambio y reinicio la API), o
  2) Actualizar el dashboard para usar un placeholder temporal para ventas (contar llamadas a `/api/venta`) hasta instrumentar `sales_total`.

Si querés, ejecuto los scripts de prueba y compruebo en Prometheus/Grafana que los panels se llenen; indícame cuál opción preferís para la medición de errores (4xx+5xx o sólo 5xx) y cuál es el endpoint exacto para ventas.
