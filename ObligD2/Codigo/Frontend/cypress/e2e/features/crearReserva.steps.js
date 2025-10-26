import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

//Escenario 1
Given('un usuario esta en la url {string}', (ReservaUrl) => {
    cy.visit(ReservaUrl);
});

Given('estoy logeado como Usuario Anonimo', () => {
});

When('ingreso el medicamento {string} y la cantidad {string}', (NombreMedicamento, CantidadMedicamento) => {
    cy.get('[id=nombreMedicamento]').select(NombreMedicamento);
    const cantidadMedicamento = parseInt(CantidadMedicamento, 10);
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

//Escenario 2
Then('se muestra un mensaje de error', () => {
    cy.get('#cantidadMedicamento')
  .invoke('prop', 'validationMessage')
  .should('contain', 'mayor o igual');
});


//Escenario 3
When('ingreso la cantidad {string} sin medicamento', (CantidadMedicamento) => {
    const cantidadMedicamento = parseInt(CantidadMedicamento, 10);
    cy.get('[id=cantidadMedicamento]').clear().type(cantidadMedicamento);
});

Then('se muestra el mensaje de error {string}', (mensajeError) => {
    cy.get("[id=errorNombre]").should("have.text", mensajeError);
});


//Escenario 4

When('no hay stock de {string}', (nombreMedicamento) => {
    cy.intercept('GET', `/api/Reserva?nombre=${nombreMedicamento}*`, {
        statusCode: 200,
        body: false,
    }).as('CheckStock');
});

Then('se muestra el mensaje de error de {string}', (mensajeError) => {
    cy.get("[id=errorStock]").should("have.text", mensajeError);
});

//Escenario 5
When('ingreso el medicamento {string} sin cantidad', (nombreMedicamento) => {
    cy.get('[id=nombreMedicamento]').select(nombreMedicamento);
    cy.get('[id=cantidadMedicamento]').clear();
});

Then('se muestra un mensaje de error {string}', (mensajeError) => {
    cy.get("[id=errorCantidad]").should("have.text", mensajeError);
});


//Escenario 6
Given('un usuario que está en la url {string}', (homeUrl) => {
    cy.visit(homeUrl);
});

Then('se redirige a la pagina {string}', (pagina) => {
    cy.url().should('include', pagina);
});

//Escenario 7
When('hago click en boton {string}', (NombreBoton) => {
    cy.get("[id=botonAtras]").should("have.text", NombreBoton).click();
});
