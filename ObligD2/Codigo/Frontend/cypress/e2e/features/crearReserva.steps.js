import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

Given('un usuario esta en la url {string}', (reservaUrl) => {
    cy.visit(reservaUrl);
});

Given('estoy logeado como Usuario Anonimo', () => {
});

When('ingreso el medicamento {string} y la cantidad {string}', (NombreMedicamento, CantidadMedicamento) => {
    const cantidadMedicamento = parseInt(CantidadMedicamento, 10);
    cy.get('[id=nombreMedicamento]').type(NombreMedicamento);
    cy.get('[id=cantidadMedicamento]').clear().type(cantidadMedicamento);
});

When('hay stock', () => {
});

When('hago click en el boton {string}', (NombreBoton) => {
    cy.get("[id=botonReservar]").should("have.text", NombreBoton).click();
});

Then('se reserva el medicamento', () => {
    cy.url().should('include', '/home');
});