Feature: Crear Reserva
    Scenario: Reservar exitosamente
        Given un usuario esta en la url "http://localhost:4200/crear-reserva"
        And estoy logeado como "Usuario Anonimo"
        When ingreso el medicamento "Ibuprofeno" y la cantidad "10"
        And hay stock de "Ibuprofeno"
        And hago click en el boton "Reservar"
        Then se reserva el medicamento con estado "Pendiente"

    