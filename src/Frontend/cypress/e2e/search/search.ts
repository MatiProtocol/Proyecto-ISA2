import { Given, When, Then, And } from 'cypress-cucumber-preprocessor/steps';

Given(
  'que hay una lista inicial de medicamentos con {string} y {string}',
  (drugName1, drugName2) => {
    const drug1 = {
      id: 1,
      code: 'PERIFAR',
      name: drugName1,
      symptom: 'headache',
      quantity: 1,
      stock: 10,
      price: 50,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: { id: 1, name: 'Pharmacy Ala' },
    };
    const drug2 = {
      id: 2,
      code: 'KALITRON',
      name: drugName2,
      symptom: 'headache',
      quantity: 1,
      stock: 5,
      price: 60,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: { id: 1, name: 'Pharmacy Ala' },
    };

    cy.intercept({ method: 'GET', url: '**/api/drug*' }, (req) => {
      if (req.url?.includes('Name=')) {
        req.reply({ statusCode: 200, body: [drug1] });
      } else {
        req.reply({ statusCode: 200, body: [drug1, drug2] });
      }
    }).as('getDrugs');

    cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] }).as('getPharmacies');

    cy.visit('http://localhost:4200/home');
    cy.wait('@getDrugs');
  },
);

And('busco por {string}', (filter) => {
  cy.get('input[placeholder="Search"]').clear().type(filter);
  cy.get('button').contains('Search').click();
  cy.wait('@getDrugs');
});

And('solo se muestra {string}', (name) => {
  cy.get('h4[cCardTitle]').should('have.length', 1).and('contain.text', name);
});

When('presiono el boton {string}', (label) => {
  cy.get('button').contains(label).click();
  cy.wait('@getDrugs');
});

Then('se muestran {string} y {string}', (name1, name2) => {
  cy.get('h4[cCardTitle]').should('have.length', 2);
  cy.contains(name1).should('exist');
  cy.contains(name2).should('exist');
});

And('el input de busqueda queda vacio', () => {
  cy.get('input[placeholder="Search"]').should('have.value', '');
});
