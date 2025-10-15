// cypress/support/e2e.ts
// Este archivo se ejecuta antes de cada prueba E2E
// Aquí puedes definir:
// - Variables globales
// - Configuraciones de mock
// - Comandos personalizados de Cypress
// - Hooks que se ejecuten antes/después de cada test

// Ejemplo de comando personalizado:
// Cypress.Commands.add('login', (username: string, password: string) => {
//   cy.visit('/login');
//   cy.get('[data-cy=username]').type(username);
//   cy.get('[data-cy=password]').type(password);
//   cy.get('[data-cy=login-button]').click();
// });

// Configuración global para timeouts, baseUrl, etc.
Cypress.config('defaultCommandTimeout', 10000);

export {};