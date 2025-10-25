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
    // Interceptar la llamada POST a la API
    cy.intercept('POST', '**/api/Reserva', {
        statusCode: 200,
        body: { message: 'Reserva creada exitosamente' }
    }).as('crearReserva');
    
    cy.get("[id=botonReservar]").should("have.text", NombreBoton).click();
});

Then('se reserva el medicamento', () => {
    cy.wait('@crearReserva');
    cy.url().should('include', '/home');
});