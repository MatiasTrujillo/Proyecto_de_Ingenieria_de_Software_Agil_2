import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

//Escenario 1
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso el medicamento {string} y la cantidad {int}', (nombreMedicamento, cantidadMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hay stock de {string}', (nombreMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se reserva el medicamento con estado {string}', (nombreEstado) => {
    cy.get("[id=nombreEstado]").should("have.text", "Pendiente");
});


//Escenario 2
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso el medicamento {string} y la cantidad {int}', (nombreMedicamento, cantidadMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('No hay stock de {string}', (nombreMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=mensajeError]").should("have.text", "No hay stock disponible");
});


//Escenario 3
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso el medicamento {string} y la cantidad {int}', (nombreMedicamento, cantidadMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=mensajeError]").should("have.text", "La cantidad debe ser mayor a 0");
});


//Escenario 4
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso el medicamento {string} y la cantidad {string}', (nombreMedicamento, cantidadMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=mensajeError]").should("have.text", "Solo caracteres numericos");
});


//Escenario 5
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso la cantidad {int} sin medicamento', (cantidadMedicamento) => {
    cy.contains(cantidadMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=mensajeError]").should("have.text", "Debe ingresar un medicamento");
});


//Escenario 6
Given('Un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('Estoy logeado como {string}', (nombreUsuario) => {
    cy.get("[id=username]").should("have.text", "Usuario Anonimo");
})

When('Ingreso el medicamento {string} sin cantidad', (nombreMedicamento) => {
    cy.contains(nombreMedicamento, { matchCase: false }).click()
});

When('Hago click en el boton {string}', (nombreBoton) => {
    cy.contains(nombreBoton, { matchCase: false }).click()
});

Then('Se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=mensajeError]").should("have.text", "Debe ingresar una cantidad");
});

