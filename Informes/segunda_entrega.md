# Informe primera entrega
- Iván Casales: 257790
- Francisco Gandioli: 290071
- Matías Trujillo: 272561

## Índice
1. [Configuración del pipeline CI/CD](#1-configuración-del-pipeline-cicd)
2. [Elección y realización de Issues](#2-elección-y-realización-de-issues)


# 1. Configuración del pipeline CI/CD
A continuación haremos una descripción de los múltiples pasos del Pipeline de forma ordenada.

## 1.1. Desarrollo (Manual)

En esta primera etapa, el desarrollador trabaja de manera local en su entorno configurado. Aquí se corrigen bugs reportados o se implementan nuevas funcionalidades siguiendo la metodología TDD (Test Driven Development).

Para mantener un control adecuado sobre el flujo de cambios, cada nueva tarea o issue se desarrolla en una rama independiente. Esto asegura que ningún cambio en progreso llegue a la rama main sin haber pasado previamente por las validaciones correspondientes.

Este enfoque favorece la trazabilidad del código, el control de versiones y la detección temprana de errores.
	
## 1.2. Commit (Manual)

Durante el desarrollo, el programador realiza commits frecuentes en su rama local, respetando las convenciones de nombres definidas por el equipo. Estas convenciones permiten identificar rápidamente el tipo de cambio realizado:
- Green: se utiliza cuando se implementa la solución mínima necesaria para que los tests pasen exitosamente.
- Refactor: se utiliza cuando se realizan mejoras al código (ya sea en estilo, estructura o eficiencia) sin alterar el comportamiento funcional ni romper los tests.

Esto asegura un historial de commits claro, ordenado y fácil de seguir durante la revisión.

## 1.3. Creación Pull Request (Manual)

Una vez finalizada la implementación de la solución, el desarrollador abre una Pull Request (PR) hacia la rama main.

Cada PR debe contener:
- Descripción detallada del problema resuelto.
- Referencia al issue vinculado, asegurando la trazabilidad entre el problema reportado y la solución.
- Evidencia de la ejecución de los tests, como capturas de pantalla o reportes generados.

El objetivo es proporcionar suficiente contexto para que los revisores comprendan el cambio y lo evalúen adecuadamente.

## 1.4. Build (Github Action - Automático)
Al momento de crear la PR, se activa automáticamente un workflow en GitHub Actions. 

Este proceso automatizado realiza las siguientes tareas:
- 1. Descarga el código del repositorio.
- 2. Compila el proyecto, asegurando que el código fuente sea válido.
- 3. Verifica las dependencias instaladas y detecta posibles errores de compilación.

De esta forma, se establece una primera capa de validación técnica antes de que el   código pueda ser revisado por otros integrantes del equipo.


## 1.5. Unit test (Github Action - Automático)

Si la compilación fue exitosa, el workflow continúa con la ejecución de todos los tests unitarios definidos en el proyecto.

El objetivo principal de esta etapa es:

- Confirmar que los cambios introducidos no rompen funcionalidades existentes.
- Validar que la implementación cumple con las reglas de TDD, es decir, que los tests fueron diseñados y aprobados antes de la integración final.

En caso de fallar algún test, la PR no puede avanzar y debe ser corregida por el desarrollador.


## 1.6. Coverage (Github Action - Automatico)

A continuación, se ejecuta un análisis de cobertura de código. Esta etapa mide qué porcentaje del código está siendo evaluado por los tests automatizados.

El reporte generado queda asociado directamente a la PR y permite:

- Validar que las áreas críticas del sistema se encuentran debidamente cubiertas.


## 1.7. Peer review PR (Manual)

Superadas las validaciones automáticas, la Pull Request pasa a revisión manual por parte del equipo.

Los reviewers analizan distintos aspectos de la solución:
- Calidad del código: claridad, legibilidad y respeto por las convenciones.
- Impacto en la arquitectura: verificación de que no se introduzcan problemas de diseño.
- Cobertura y calidad de tests: confirmación de que los escenarios críticos estén cubiertos.
- Resolución del issue: se valida que el problema reportado quede efectivamente solucionado.

En esta etapa se pueden sugerir mejoras o solicitar cambios adicionales antes de aprobar la integración.

## 1.8. Aceptar PR + merge (Manual)
Cuando la Pull Request cumple con todos los requisitos, un reviewer autorizado aprueba la integración y procede a realizar el merge a la rama main.

Al completar este paso se realizan tareas complementarias para mantener el proyecto organizado:

- Se actualiza el tablero Kanban, reflejando el avance del flujo de trabajo.
- Se cierra el issue correspondiente, asegurando la trazabilidad del ciclo de desarrollo.

Este proceso garantiza que cada entrega pase por validaciones automáticas y manuales antes de llegar a la rama principal.

![alt text](<images/image2.jpg>)


# 2. Elección y realización de Issues
## 2.1. Creado de administrador
**Descripción del problema**: 
Para que la aplicación pueda ser utilizada correctamente, necesita tener un usuario administrador creado, el cual no se crea automáticamente.

**Justificación de selección en base al modelo de calidad**: 
Decidimos empezar por este problema, ya que es necesario solucionarlo para ejecutar la aplicación por primera vez.

Este problema corresponde al atributo de Funcionalidad dentro del modelo de calidad, porque sin la creación automática del usuario administrador el sistema no puede usarse correctamente desde el inicio. Es decir, falta una función esencial para que la aplicación cumpla con lo esperado y sea operativa desde la primera ejecución.

**Solución**: Cada vez que la aplicación se inicia, se controla que exista un usuario admin, de lo contrario, se crea uno en la base de datos.

## 2.2. Falta de acceso al login/registro
**Descripción del problema**: Para que la página sea funcional, y pueda llevar un registro de quien compro que, es necesario tener la capacidad de iniciar sesión (o registrarse de ser necesario) a la hora de hacer la aplicación. Dicha funcionalidad ya está implementada, pero hasta ahora para acceder a la misma había que modificar la url de la página manualmente, sin existir un botón para acceder directamente.

**Justificación de selección en base al modelo de calidad**: 
Decidimos arreglar este problema debido a que es fundamental a la hora de acceder a una página web, el hecho de poder logearse/registrarse en ella. Creemos que no es necesario el hecho de que el login/registro sea la primera ventana que se ve al abrir la página debido a la existencia de varias paginas (como mercado libre o amazon) en la que se puede visualizar directamente los productos antes de registrarte.

**Solución**: En la ventana principal se puede ver agregado los botones de logueo y de registro respectivamente, uno a lado del otro. Estos botones tienen asociados la página correspondiente.

### Nota
Si bien no lo consideramos como parte de las issues seleccionadas, tuvimos que realizar la siguiente issue debido a los parámetros configurados en las Github Actions:
Test Get_One_Detail_By_Expression falla

El test Get_One_Detail_By_Expression fallaba, por lo que tuvimos que arreglar el test para que las pull requests puedan ser aceptadas por el pipeline y mantener la consistencia de que todos los test deben pasar para poder realizar la integración en main.
