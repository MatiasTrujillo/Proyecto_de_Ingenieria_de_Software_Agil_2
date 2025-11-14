# Informe cuarta entrega
- Iván Casales: 257790  
- Francisco Gandioli: 290071  
- Matías Trujillo: 272561  

## Índice

1. [Implementar nuevo tipo de testing](#1-implementar-nuevo-tipo-de-testing)
2. [Cálculo, justificación e interpretación de métricas DevOps](#2-cálculo-justificación-e-interpretación-de-métricas-devops)
3. [Implementación de telemetría](#3-implementación-de-telemetría)


# 1. Implementar nuevo tipo de testing

Con el objetivo de fortalecer la seguridad del código fuente del proyecto, se incorporó un nuevo tipo de prueba: análisis estático de seguridad (SAST) mediante la herramienta Semgrep. Este tipo de testing analiza el código sin ejecutarlo, buscando patrones potencialmente vulnerables según reglas predefinidas.

Semgrep es una herramienta de análisis estático moderna que combina análisis semántico (AST) con búsqueda basada en patrones. Su propósito es detectar constructos de código que puedan derivar en vulnerabilidades de seguridad o malas prácticas de desarrollo.

- Ventajas principales:
    - Rápido y ligero
    - Lenguaje-consciente
    - Personalizable
    - Integrable en CI/CD
- Limitaciones:
    - No detecta vulnerabilidades que solo aparecen en tiempo de ejecución (por ejemplo, race conditions o errores de configuración dinámica).
    - Algunas reglas genéricas pueden generar falsos positivos si no se ajustan adecuadamente.
    - No reemplaza pruebas dinámicas (DAST) ni auditorías manuales, sino que las complementa.

### Evidencia de ejecución
Al ejecutar Semgrep sobre el código del proyecto, el resultado del análisis (archivo SARIF generado, semg	rep-results) reportó múltiples hallazgos asociados a la regla hardcoded-password, que identifica contraseñas o credenciales escritas directamente en el código fuente.

Ejemplos concretos:

En el archivo DatabaseInitializationService.cs se detectó la línea:

 Password = "Admin123!"; // En producción, esto debería estar hasheado

 → Riesgo: exposición de credenciales en texto plano.

Estos hallazgos son relevantes ya que el uso de credenciales hardcodeadas puede permitir el acceso no autorizado, comprometer datos sensibles o generar malas prácticas replicadas en entornos productivos.

La incorporación de Semgrep como herramienta SAST permitió introducir un nuevo tipo de testing orientado a la seguridad del código fuente, alineado con las prácticas modernas de desarrollo seguro.

El análisis detectó vulnerabilidades reales (contraseñas hardcodeadas) y permitió definir mejoras para fortalecer la gestión de credenciales, cumpliendo con los objetivos de calidad del proyecto y aportando una capa adicional de prevención de riesgos de seguridad antes del despliegue.

# 2. Cálculo, justificación e interpretación de métricas DevOps
### Change Lead Time:

Esta métrica mide el tiempo que tarda un commit en llegar a producción. Debido a que no tenemos despliegues en este proyecto, tomaremos el tiempo que tarda una Github Action en correr (las cuales se ejecutan cuando se hace un pull request).

Haciendo un promedio de cuanto tardaron las Github Actions de los merge correctos tenemos un Change Lead Time de: (77+131+86+84+81+79+90+91+92+85+96) / 11 = 90 segundos. 

Esto nos dice que nuestro Change Lead Time es rápido, lo que logra una entrega rápida de valor (o en este caso, una entrega rápida de cambios a nuestro equipo). 

En nuestro caso, aunque existen algunos valores algo más altos (como los 131 segundos), la mayoría de los tiempos se concentran alrededor del promedio (entre 79 y 96 s), lo que implica que el proceso está bajo control y no presenta variaciones significativas.

### Change Fail Percentage:
Esta métrica mide el porcentaje de despliegues fallidos. Una vez más, como no tenemos despliegues, decidimos adaptar esta métrica a nuestro proyecto. Decidimos que contaremos las Github Actions fallidas como despliegues fallidos.

En este caso, observamos que nuestro Change Fail Percentage es de: 6/17 = 0,35 = 35%. 

Este valor indica un porcentaje alto de errores. Sin embargo, la gran mayoría de estos errores (5 de los 6 errores) fueron al principio del proyecto, cuando aún estábamos configurando las Github Actions y familiarizándonos con el proyecto. 

# 3. Implementación de telemetría
- Métricas de negocio
    - Ventas mensuales: cantidad total de ventas registradas por mes.
        - Permite evaluar el desempeño comercial general del sistema.
    - Reservas mensuales: cantidad de reservas realizadas por mes.
        - Ayuda a medir la demanda y el uso real del servicio.

- Métricas de aplicación
    - Tiempo de respuesta de reserva: mide cuánto tarda la aplicación en responder cuando el usuario realiza una reserva (desde que el usuario hace click en "Reservar" hasta que se confirma la reserva).
        - Indica la rapidez y eficiencia percibida por el usuario.
    - Tasa de errores: porcentaje de solicitudes de reserva que fallan.
        - Permite detectar fallas o inestabilidad en el sistema.

- Métricas de infraestructura
    - Uso promedio de CPU del contenedor: porcentaje de uso del procesador en el contenedor Docker.
        - Ayuda a identificar sobrecargas o necesidad de optimizar recursos.
    - Uso promedio de memoria del contenedor: cantidad de memoria utilizada por el contenedor Docker.
        - Permite controlar el consumo y prevenir posibles fallas por falta de recursos.
