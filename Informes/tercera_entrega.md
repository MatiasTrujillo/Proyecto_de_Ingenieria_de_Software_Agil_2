# Informe tercera entrega
- Iván Casales: 257790  
- Francisco Gandioli: 290071  
- Matías Trujillo: 272561  

## Índice
1. [Creado de User Stories](#1-creado-de-user-stories)
2. [Aclaraciones](#2-aclaraciones)



# 1. Creado de User Stories

---

## 1.1. Crear la reserva de un medicamento 

**Título:** Crear la reserva de un medicamento  

**Narrativa:**  
> Como usuario  
> quiero poder crear una reserva de medicamento  
> para asegurarme que tenga lo que necesito asegurado

**Preguntas:**  
1. ¿Qué es una reserva? Una forma de asegurarme de que exista un medicamento cuando llegue al local a retirarlo y pagarlo.  
2. ¿Quién la puede hacer? Un cliente (“usuario anónimo”).  
3. ¿Qué información debe ingresar el cliente? Tipo de medicamento y cantidad del medicamento.  

**Reglas:**  
1. Tiene que haber stock del medicamento para reservarse.  
2. El stock ingresado debe ser mayor a 0.  
3. El usuario debe ingresar un medicamento.  
4. El usuario debe ingresar una cantidad.  
5. El usuario accede a la página desde “Home”.

**Escenarios (Given/When/Then):**
- **Stock disponible:**  
  - Given un usuario en “/reservarMedicamento”  
  - When ingresa “Ibuprofeno”, cantidad “10”  
  - Then se reserva con estado “Pendiente”.
- **Sin stock:**  
  - Given un usuario en “/reservarMedicamento”  
  - When ingresa “Ibuprofeno”, cantidad “10” sin stock  
  - Then se muestra “No hay stock disponible”.  
- **Cantidad ≤ 0:**  
  - Given un usuario intenta reservar con cantidad ≤ 0  
  - Then el sistema no lo permite.  
- **Sin medicamento:**  
  - Given un usuario sin medicamento  
  - Then se muestra “Debe ingresar un medicamento”.  
- **Sin cantidad:**  
  - Given un usuario sin cantidad  
  - Then se muestra “Debe ingresar una cantidad”.  
- **Navegación:**  
  - From `/home` → click “Reservar” → redirige a `/reservarMedicamento`.  
  - From `/reservarMedicamento` → click “Atrás” → redirige a `/home`.

---

## 1.2. Gestionar reservas de un medicamento 

**Título:** Gestionar reservas de un medicamento  

**Narrativa:**  
> Como usuario  
> quiero poder ver las reservas que hice  
> para saber si me faltó algo o tengo todo  

**Preguntas:**  
1. ¿Qué significa “gestionar”? Ver una lista de reservas con su información y opción de cancelar.  
2. ¿Qué información tiene una reserva? Medicamento, cantidad y fecha de expiración.  

**Regla:**  
1. El usuario puede ver la lista con información y botón de cancelar.

**Escenario:**  
- Given un usuario que quiere gestionar sus reservas  
- When hace click en “Gestionar reservas”  
- Then se muestra la lista de reservas.

---

## 1.3. Cancelar una reserva de medicamento 

**Título:** Cancelar una reserva de medicamentos  

**Narrativa:**  
> Como usuario  
> quiero poder cancelar las reservas  
> para sacar los medicamentos que no quiero de la lista  

**Preguntas:**  
1. ¿Qué implica cancelar? El sistema elimina la reserva de la lista.  

**Regla:**  
1. No se puede cancelar una reserva confirmada.  

**Escenarios:**  
- **Reserva confirmada:** no muestra botón “Cancelar”.  
- **Reserva pendiente:** se permite cancelar.

---

## 1.4. Confirmar reserva de medicamentos 

**Título:** Confirmar reserva de medicamentos (empleado)  

**Narrativa:**  
> Como empleado  
> quiero poder confirmar las reservas de los usuarios  
> para verificar que todos los requisitos se cumplan  

**Preguntas:**  
1. ¿Hay algo más que confirmar además de la prescripción? No.  

**Reglas:**  
1. Si tiene prescripción, deben cumplirse los requisitos antes de confirmar.  
2. Si no tiene prescripción, se confirma directamente.  

**Escenarios:**  
- **Con prescripción válida:** se confirma la reserva.  
- **Con prescripción faltante:** no se confirma.  
- **Sin prescripción requerida:** se confirma automáticamente.

---

# 2. Aclaraciones

1. Se modificó la estructura de carpetas y archivos `cypress.config.ts`, `package.json` y `tsconfig.json` para compatibilidad con Cypress.  
2. El escenario 2 de *Crear reserva* puede fallar si el idioma del entorno no es español.  
3. Las pruebas Cypress de creación de reserva requieren una droga llamada **"Droga1"** con stock **200**.  



## Diagramas de Secuencia y Componentes

### 🧩 Diagrama de Secuencia — Gestión de Reserva
![Diagrama de Gestión de Reserva](images/Diagrama%20de%20Gestion.drawio.png)

### 🧩 Diagrama de Secuencia — Cancelación de Reserva
![Diagrama de Cancelación de Reserva](images/Diagrama%20de%20Cancelacion.drawio.png)

### 🧱 Diagrama de Componentes
![Diagrama de Componentes](images/Diagrama%20de%20Componentes.drawio.png)



