Feature: Confirmar Reservas (Empleado)
  Como empleado de la farmacia
  Quiero confirmar reservas de medicamentos de los usuarios
  Para verificar que tienen prescripción cuando sea necesario

  Scenario: Empleado confirma reserva al haber prescripción del medicamento
    Given un empleado está en la página de confirmar reservas
    And existe una reserva que requiere prescripción
    When el empleado hace click en "Confirmar con Prescripción"
    Then la reserva es confirmada exitosamente
    And se muestra el mensaje "Reserva confirmada exitosamente"
    And la reserva desaparece de la lista de pendientes

  Scenario: Empleado no confirma reserva al no haber prescripción del medicamento
    Given un empleado está en la página de confirmar reservas
    And existe una reserva que requiere prescripción
    When el empleado hace click en "Confirmar sin Prescripción"
    Then se muestra el mensaje "No se puede confirmar. El medicamento requiere prescripción"
    And la reserva permanece en la lista de pendientes

  Scenario: Empleado confirma reserva al no necesitar prescripción del medicamento
    Given un empleado está en la página de confirmar reservas
    And existe una reserva que no requiere prescripción
    When el empleado hace click en "Confirmar sin Prescripción"
    Then la reserva es confirmada exitosamente
    And se muestra el mensaje "Reserva confirmada exitosamente"
    And la reserva desaparece de la lista de pendientes
