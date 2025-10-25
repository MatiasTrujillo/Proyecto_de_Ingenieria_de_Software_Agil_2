Feature: Crear Reserva
    Scenario: Reservar exitosamente
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Ibuprofeno" y la cantidad "10"
        And hay stock
        And hago click en el boton " Reservar "
        Then se reserva el medicamento

        Scenario: Intentar reservar una cantidad menor o igual a 0
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Ibuprofeno" y la cantidad "0"
        And hago click en el boton " Reservar "
        Then se muestra el mensaje de error "La cantidad debe ser mayor o igual a 1"

    