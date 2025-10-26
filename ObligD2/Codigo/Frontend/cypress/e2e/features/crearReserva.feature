Feature: Crear Reserva
    Scenario: Reservar exitosamente
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Droga1" y la cantidad "10"
        And hay stock
        And hago click en el boton " Reservar "
        Then se reserva el medicamento

    Scenario: Intentar reservar una cantidad menor o igual a 0
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Droga1" y la cantidad "0"
        And hago click en el boton " Reservar "
        Then se muestra un mensaje de error

    Scenario: Reservar sin ingresar medicamento
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso la cantidad "10" sin medicamento
        And hago click en el boton " Reservar "
        Then se muestra el mensaje de error "Por favor seleccione un medicamento"

    Scenario: Intentar reservar sin stock
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Droga1" y la cantidad "1000"
        And no hay stock de "Droga1"
        And hago click en el boton " Reservar "
        Then se muestra el mensaje de error de "No hay stock disponible"

    Scenario: Reservar sin ingresar cantidad
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como Usuario Anonimo
        When ingreso el medicamento "Droga1" sin cantidad
        And hago click en el boton " Reservar "
        Then se muestra un mensaje de error "Debe ingresar una cantidad"
    