import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

Given('un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('estoy logeado como {string}', (NombreUsuario) => {
    //cy.get("[id=username]").should("have.text", "Usuario Anonimo");
});

When('ingreso el medicamento {string} y la cantidad {int}', (NombreMedicamento, CantidadMedicamento) => {
    cy.contains(NombreMedicamento, { matchCase: false }).click()
});

When('hay stock de {string}', (NombreMedicamento) => {
    //cy.contains(NombreMedicamento, { matchCase: false }).click()
});

When('hago click en el boton {string}', (NombreBoton) => {
    cy.contains(NombreBoton, { matchCase: false }).click()
});

Then('se reserva el medicamento con estado {string}', (NombreEstado) => {
    //cy.get("[id=nombreEstado]").should("have.text", "Pendiente");
});