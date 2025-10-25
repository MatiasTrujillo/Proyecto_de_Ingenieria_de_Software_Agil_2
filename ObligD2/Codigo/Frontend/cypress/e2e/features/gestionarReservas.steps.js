import { Given, When, Then } from '@badeball/cypress-cucumber-preprocessor';

// Scenario: Ver lista de reservas vacía
Given('un usuario está en la página de gestionar reservas', () => {
  // No visitamos todavía, solo indicamos la intención
  // La visita se hará después de configurar los interceptores
});

Given('no hay reservas creadas', () => {
  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: []
  }).as('getReservasEmpty');
  
  cy.visit('http://localhost:4200/gestionar-reservas');
});

When('la página carga', () => {
  cy.wait(500); // Esperar a que la página termine de cargar
});

Then('se muestra el mensaje {string}', (mensaje) => {
  cy.contains(mensaje).should('be.visible');
});

Then('se muestra el botón {string}', (textoBoton) => {
  cy.contains('button', textoBoton).should('be.visible');
});

// Scenario: Ver lista de reservas con datos
Given('existen reservas en el sistema', () => {
  const reservas = [
    {
      id: 1,
      nombreMedicamento: 'Ibuprofeno',
      cantidadMedicamento: 10,
      fechaExpiracion: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(), // 7 días en el futuro
      fechaReserva: new Date().toISOString(),
      usuario: 'Usuario Test'
    },
    {
      id: 2,
      nombreMedicamento: 'Paracetamol',
      cantidadMedicamento: 5,
      fechaExpiracion: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000).toISOString(), // 10 días en el futuro
      fechaReserva: new Date().toISOString(),
      usuario: 'Usuario Test'
    }
  ];

  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: reservas
  }).as('getReservas');
  
  cy.visit('http://localhost:4200/gestionar-reservas');
});

Then('se muestra una tabla con las reservas', () => {
  cy.wait('@getReservas', { timeout: 10000 }); // Esperar a que se carguen las reservas
  cy.get('#tablaReservas').should('be.visible');
  cy.get('#tablaReservas tbody tr').should('have.length.at.least', 1);
});

Then('cada reserva muestra el nombre del medicamento', () => {
  cy.get('#tablaReservas tbody tr').first().should('contain', 'Ibuprofeno');
});

Then('cada reserva muestra la cantidad', () => {
  cy.get('#tablaReservas tbody tr').first().should('contain', '10 unidades');
});

Then('cada reserva muestra la fecha de expiración', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(4).should('exist').and('not.be.empty');
});

Then('cada reserva tiene un botón de cancelar', () => {
  cy.get('.btnCancelar').should('have.length.at.least', 1);
});

// Scenario: Cancelar una reserva exitosamente
When('hace click en el botón {string} de una reserva', (textoBoton) => {
  // Esperar a que la tabla esté visible primero
  cy.get('#tablaReservas').should('be.visible');
  cy.get('.btnCancelar').should('have.length.at.least', 1);
  
  // Interceptar la llamada DELETE
  cy.intercept('DELETE', '**/api/Reserva/1', {
    statusCode: 200,
    body: { message: 'Reserva cancelada exitosamente' }
  }).as('deleteReserva');

  // Interceptar la recarga después del delete
  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: [
      {
        id: 2,
        nombreMedicamento: 'Paracetamol',
        cantidadMedicamento: 5,
        fechaExpiracion: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000).toISOString(),
        fechaReserva: new Date().toISOString(),
        usuario: 'Usuario Test'
      }
    ]
  }).as('getReservasAfterDelete');

  cy.get('.btnCancelar').first().click();
});

When('confirma la cancelación', () => {
  // Cypress automáticamente acepta los confirm()
  cy.on('window:confirm', () => true);
});

Then('la reserva es eliminada', () => {
  cy.wait('@deleteReserva');
  cy.wait('@getReservasAfterDelete');
});

Then('la reserva ya no aparece en la lista', () => {
  cy.get('#tablaReservas tbody tr').should('have.length', 1);
  cy.get('#tablaReservas').should('not.contain', 'Ibuprofeno');
});

// Scenario: Ir a crear nueva reserva desde gestionar
When('hace click en el botón {string}', (textoBoton) => {
  if (textoBoton === 'Nueva Reserva') {
    cy.get('#btnCrearReserva').click();
  } else if (textoBoton === 'Volver') {
    cy.get('#btnVolver').click();
  }
});

Then('es redirigido a la página de crear reserva', () => {
  cy.url().should('include', '/crear-reserva');
});

// Scenario: Volver al home desde gestionar reservas
Then('es redirigido a la página de home', () => {
  cy.url().should('include', '/home');
});

// Scenario: Ver reserva expirada
Given('existe una reserva con fecha de expiración pasada', () => {
  const reservaExpirada = {
    id: 3,
    nombreMedicamento: 'Aspirina',
    cantidadMedicamento: 8,
    fechaExpiracion: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(), // 2 días atrás
    fechaReserva: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
    usuario: 'Usuario Test'
  };

  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: [reservaExpirada]
  }).as('getReservaExpirada');
  
  cy.visit('http://localhost:4200/gestionar-reservas');
});

Then('la reserva se muestra con el estado {string}', (estado) => {
  cy.wait('@getReservaExpirada', { timeout: 10000 });
  cy.get('#tablaReservas tbody tr').first().should('contain', estado);
});

Then('el botón de cancelar está deshabilitado', () => {
  cy.get('.btnCancelar').first().should('be.disabled');
});

// Scenario: Ver información completa de reserva
Given('existe una reserva con todos sus datos', () => {
  const reservaCompleta = {
    id: 1,
    nombreMedicamento: 'Ibuprofeno',
    cantidadMedicamento: 10,
    fechaExpiracion: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    fechaReserva: new Date().toISOString(),
    usuario: 'Usuario Test'
  };

  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: [reservaCompleta]
  }).as('getReservaCompleta');
  
  cy.visit('http://localhost:4200/gestionar-reservas');
});

Then('se muestra el ID de la reserva', () => {
  cy.wait('@getReservaCompleta', { timeout: 10000 });
  cy.get('#tablaReservas tbody tr').first().find('td').eq(0).should('contain', '1');
});

Then('se muestra el nombre del medicamento', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(1).should('contain', 'Ibuprofeno');
});

Then('se muestra la cantidad de medicamento', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(2).should('contain', '10');
});

Then('se muestra la fecha de reserva', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(3).should('exist');
});

Then('se muestra la fecha de expiración', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(4).should('exist').and('not.be.empty');
});

Then('se muestra el usuario que la creó', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(5).should('contain', 'Usuario Test');
});

Then('se muestra el estado de la reserva', () => {
  cy.get('#tablaReservas tbody tr').first().find('td').eq(6).should('contain', 'Activa');
});
