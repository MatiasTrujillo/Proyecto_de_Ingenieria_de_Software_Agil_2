Feature: Crear Reserva
    Scenario: Reservar exitosamente
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso el medicamento "Ibuprofeno" y la cantidad "10"
        And hay stock de "Ibuprofeno"
        And hago click en el boton "Reservar"
        Then se reserva el medicamento con estado "Pendiente"

    Scenario: Intentar reservar sin stock
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso el medicamento "Ibuprofeno" y la cantidad "100"
        And no hay stock de "Ibuprofeno"
        And hago click en el boton "Reservar"
        Then se muestra el mensaje de error "No hay stock disponible"

    Scenario: Intentar reservar una cantidad menor o igual a 0
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso el medicamento "Ibuprofeno" y la cantidad "0"
        And hago click en el boton "Reservar"
        Then se muestra el mensaje de error "La cantidad debe ser mayor a 0"

    Scenario: Reservar con caracteres no numericos en la cantidad
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso el medicamento "Ibuprofeno" y la cantidad "cinco"
        And hago click en el boton "Reservar"
        Then se muestra el mensaje de error "Solo caracteres numericos"

    Scenario: Reservar sin ingresar medicamento
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso la cantidad "10" sin medicamento
        And hago click en el boton "Reservar"
        Then se muestra el mensaje de error "Debe ingresar un medicamento"
        
    Scenario: Reservar sin ingresar cantidad
        Given Un usuario esta en la url "http://localhost:4200/reservarMediamento"
        And estoy logeado como "Usaurio Anonimo"
        When ingreso el medicamento "Ibuprofeno" sin cantidad
        And hago click en el boton "Reservar"
        Then se muestra el mensaje de error "Debe ingresar una cantidad"
