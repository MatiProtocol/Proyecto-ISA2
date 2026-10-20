import { Given, When, Then, And } from 'cypress-cucumber-preprocessor/steps';

Given('historial de tracking vacío', () => {
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] }).as('getPharmacies');
  cy.intercept(
    { method: 'GET', url: '**/api/purchases/tracking*' },
    { statusCode: 200, body: null },
  ).as('getPurchase');

  cy.window().then((win) => {
    win.localStorage.setItem('tracking', JSON.stringify([]));
  });

  cy.visit('http://localhost:4200/home/tracking');
  cy.wait('@getPharmacies');
});

When('presiono el boton "Track your purchase" con el input vacío', () => {
  cy.get('#CodeInput').clear();
  cy.get('button').contains('Track your purchase').click();
});

Then('no se muestra la sección "Last 5 tracked codes"', () => {
  cy.contains('Last 5 tracked codes').should('not.exist');
});

Given('historial de tracking con {string}', (code) => {
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] }).as('getPharmacies');

  cy.window().then((win) => {
    win.localStorage.setItem('tracking', JSON.stringify([code]));
  });

  cy.visit('http://localhost:4200/home/tracking');
  cy.wait('@getPharmacies');

  cy.contains('th', 'Tracking Code').closest('table').find('tbody tr').should('have.length', 1);
  cy.contains(code).should('exist');
});

And('la consulta por código {string} retorna sin resultado', (badCode) => {
  cy.intercept('GET', `**/api/purchases/tracking*${badCode}*`, { statusCode: 200, body: null }).as(
    'getPurchaseByCode',
  );
});

When('ingreso {string} y presiono el boton "Track your purchase"', (code) => {
  cy.get('#CodeInput').clear().type(code);
  cy.get('button').contains('Track your purchase').click();
  cy.wait('@getPurchaseByCode');
});

Then('la tabla de recientes contiene solo {string}', (code) => {
  cy.contains('th', 'Tracking Code').closest('table').find('tbody tr').should('have.length', 1);
  cy.contains('td', code).should('exist');
});
