Feature: Gestionar Reservas
  Como usuario
  Quiero poder ver las reservas que hice
  Para saber si me falta algo o tengo todo

  Scenario: Ver lista de reservas vacía
    Given un usuario está en la página de gestionar reservas
    And no hay reservas creadas
    When la página carga
    Then se muestra el mensaje "No tienes reservas"
    And se muestra el botón "Crear primera reserva"

  Scenario: Ver lista de reservas con datos
    Given un usuario está en la página de gestionar reservas
    And existen reservas en el sistema
    When la página carga
    Then se muestra una tabla con las reservas
    And cada reserva muestra el nombre del medicamento
    And cada reserva muestra la cantidad
    And cada reserva muestra la fecha de expiración
    And cada reserva tiene un botón de cancelar

  Scenario: Cancelar una reserva exitosamente
    Given un usuario está en la página de gestionar reservas
    And existen reservas en el sistema
    When hace click en el botón "Cancelar" de una reserva
    And confirma la cancelación
    Then la reserva es eliminada
    And la reserva ya no aparece en la lista

  Scenario: Ir a crear nueva reserva desde gestionar
    Given un usuario está en la página de gestionar reservas
    And no hay reservas creadas
    When hace click en el botón "Nueva Reserva"
    Then es redirigido a la página de crear reserva

  Scenario: Volver al home desde gestionar reservas
    Given un usuario está en la página de gestionar reservas
    And no hay reservas creadas
    When hace click en el botón "Volver"
    Then es redirigido a la página de home

  Scenario: Ver reserva expirada
    Given un usuario está en la página de gestionar reservas
    And existe una reserva con fecha de expiración pasada
    When la página carga
    Then la reserva se muestra con el estado "Expirada"
    And el botón de cancelar está deshabilitado

  Scenario: Ver información completa de reserva
    Given un usuario está en la página de gestionar reservas
    And existe una reserva con todos sus datos
    When la página carga
    Then se muestra el ID de la reserva
    And se muestra el nombre del medicamento
    And se muestra la cantidad de medicamento
    And se muestra la fecha de reserva
    And se muestra la fecha de expiración
    And se muestra el usuario que la creó
    And se muestra el estado de la reserva
