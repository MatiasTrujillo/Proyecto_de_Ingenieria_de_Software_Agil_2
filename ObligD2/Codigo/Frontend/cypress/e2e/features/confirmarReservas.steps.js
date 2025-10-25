import { Given, When, Then, Before } from '@badeball/cypress-cucumber-preprocessor';

let reservaPendiente;

Before(() => {
  // Simular login de empleado antes de cada escenario
  cy.visit('http://localhost:4200');
  cy.window().then((win) => {
    const empleadoLogin = {
      token: 'empleado-token-123',
      userName: 'empleado@test.com',
      role: 'Employee'
    };
    // IMPORTANTE: El AuthenticationGuard busca la clave "login" en localStorage
    win.localStorage.setItem('login', JSON.stringify(empleadoLogin));
  });
});

// Scenario: Empleado confirma reserva al haber prescripción del medicamento
Given('un empleado está en la página de confirmar reservas', () => {
  // Configurar datos mockeados
  reservaPendiente = {
    id: 1,
    nombreMedicamento: 'Antibiótico XYZ',
    cantidadMedicamento: 5,
    fechaExpiracion: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    fechaReserva: new Date().toISOString(),
    usuario: 'usuario@test.com',
    confirmada: false,
    requierePrescripcion: true,
    tienePrescripcion: false,
    empleadoConfirmacion: null,
    fechaConfirmacion: null
  };

  const reservas = [reservaPendiente];

  // Interceptar ANTES de visitar
  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: reservas
  }).as('getReservas');
  
  cy.visit('http://localhost:4200/employee/confirmar-reservas');
  cy.wait('@getReservas');
});

Given('existe una reserva que requiere prescripción', () => {
  // Los datos ya se configuraron en el step anterior
  // Verificar que hay al menos una fila en la tabla
  cy.get('table tbody tr').should('have.length.at.least', 1);
});

When('el empleado hace click en {string}', (textoBoton) => {
  if (textoBoton === 'Confirmar con Prescripción') {
    const reservaConfirmada = {
      ...reservaPendiente,
      confirmada: true,
      tienePrescripcion: true,
      empleadoConfirmacion: 'empleado@test.com',
      fechaConfirmacion: new Date().toISOString()
    };

    cy.intercept('PATCH', '**/api/Reserva/*/confirmar', {
      statusCode: 200,
      body: reservaConfirmada
    }).as('confirmarReserva');

    cy.intercept('GET', '**/api/Reserva', {
      statusCode: 200,
      body: []
    }).as('getReservasAfterConfirm');

    // Buscar y hacer click en el PRIMER botón "Con Prescripción"
    cy.get('button').contains('Con Prescripción').first().click();
  } else if (textoBoton === 'Confirmar sin Prescripción') {
    // Para el escenario 3, los intercepts ya están configurados en el Given
    // Solo hacer click en el botón
    cy.get('button').contains('Sin Prescripción').first().click();
  }
});

Then('la reserva es confirmada exitosamente', () => {
  cy.wait('@confirmarReserva');
  cy.wait('@getReservasAfterConfirm');
  // Esperar un momento para que el componente procese la respuesta
  cy.wait(500);
});

// NOTA: El step "se muestra el mensaje {string}" se reutiliza de gestionarReservas.steps.js
// No redefinir aquí para evitar duplicados

Then('la reserva desaparece de la lista de pendientes', () => {
  // Verificar que no hay reservas o que la tabla está vacía
  cy.get('body').then(($body) => {
    // Puede mostrar mensaje de "No hay reservas" o la tabla puede estar vacía
    if ($body.find('table tbody tr').length > 0) {
      cy.get('table tbody tr').should('have.length', 0);
    } else {
      cy.contains('No hay reservas pendientes').should('be.visible');
    }
  });
});

// Scenario: Empleado no confirma reserva al no haber prescripción del medicamento
Then('la reserva permanece en la lista de pendientes', () => {
  // Verificar que sigue habiendo reservas en la tabla
  cy.get('table tbody tr').should('have.length.at.least', 1);
});

// Scenario: Empleado confirma reserva al no necesitar prescripción del medicamento
Given('existe una reserva que no requiere prescripción', () => {
  reservaPendiente = {
    id: 2,
    nombreMedicamento: 'Vitamina C',
    cantidadMedicamento: 10,
    fechaExpiracion: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    fechaReserva: new Date().toISOString(),
    usuario: 'usuario@test.com',
    confirmada: false,
    requierePrescripcion: false,
    tienePrescripcion: false,
    empleadoConfirmacion: null,
    fechaConfirmacion: null
  };

  const reservas = [reservaPendiente];

  // Primer intercept: cuando la página carga, retorna la reserva
  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: reservas
  }).as('getReservas');

  cy.visit('http://localhost:4200/employee/confirmar-reservas');
  cy.wait('@getReservas');
  
  // Verificar que hay al menos una fila en la tabla
  cy.get('table tbody tr').should('have.length.at.least', 1);
  
  // Ahora configurar los intercepts para después del click
  const reservaConfirmada = {
    ...reservaPendiente,
    confirmada: true,
    tienePrescripcion: false,
    empleadoConfirmacion: 'empleado@test.com',
    fechaConfirmacion: new Date().toISOString()
  };

  cy.intercept('PATCH', '**/api/Reserva/*/confirmar', {
    statusCode: 200,
    body: reservaConfirmada
  }).as('confirmarReserva');

  // Este intercept se ejecutará DESPUÉS del click
  cy.intercept('GET', '**/api/Reserva', {
    statusCode: 200,
    body: []
  }).as('getReservasAfterConfirm');
});
