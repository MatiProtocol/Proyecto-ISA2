import { Given, And, When, Then } from 'cypress-cucumber-preprocessor/steps';

const bookerEmail = 'user@example.com';
const bookingDate = new Date().setDate(new Date().getDate() - 1);
const confirmationDate = new Date().setDate(new Date().getDate() - 0.5);
const publicKey = 'publicKeyBase64';
const privateKey =
  'V8vZzH+ECFr+LDqRyAnPdpyabGAPZf5mppq2pQNiZzoyZsvx7tE7yV2POE/DFL9w/Oco4KD+d/eGXX4WXGmglA==';
let reusableResponse: any;

const details = [
  {
    id: 1,
    code: 'PERIFAR',
    name: 'Perifar',
    quantity: 2,
    price: 50,
    pharmacyId: 1,
    pharmacyName: 'Pharmacy Ala',
  },
  {
    id: 2,
    code: 'KALITRON',
    name: 'Kalitron',
    quantity: 1,
    price: 50,
    pharmacyId: 1,
    pharmacyName: 'Pharmacy Ala',
  },
];

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

And('tengo una reserva de id {string} en estado {string}', (id, state) => {
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] }).as('getPharmacies');
  cy.intercept('GET', '**/api/drug**', { statusCode: 200, body: [] }).as('getDrugs');
  cy.visit('http://localhost:4200/home');
  cy.wait('@getPharmacies');
  cy.wait('@getDrugs');
  cy.get('a[id="btn-pick-up"]').click();
  cy.window().url().should('include', '/home/bookings/pick-up');

  cy.intercept('GET', `/api/booking/${id}`, (req) => {
    req.reply({
      statusCode: 200,
      body: {
        id: id,
        bookerEmail: bookerEmail,
        bookingDate: bookingDate,
        confirmationDate: confirmationDate,
        closureDate: '',
        publicKey: publicKey,
        status: state,
        totalAmount: details.reduce((total, item) => total + item.price * item.quantity, 0),
        details: details,
      },
    });
  }).as('getBooking');

  cy.get('button[id="search-booking-pick-btn"]').click();

  cy.wait('@getBooking');
  cy.get('p[id="booking-status"]').should('be.visible');
  cy.get('p[id="booking-status"]').should('contain', state);
});

When('presento la clave privada para la reserva de id "1"', () => {
  cy.get('input[id="private-key-input"]').should('be.visible').type(privateKey);
});

And('estamos a "12.12.2026" a las "12:00"', () => {
  const now = new Date(2026, 11, 12, 12, 0, 0);
  cy.clock(now.getTime());
});

And('quiero retirarla', () => {
  cy.intercept('POST', `/api/booking/1/challenge`, (req) => {
    req.reply({
      statusCode: 200,
      body: {
        challenge: 'challengeString',
      },
    });
  }).as('postChallenge');

  cy.intercept('POST', `/api/booking/1/challenge-confirm`, (req) => {
    const { challenge, signature } = req.body;
    expect(challenge).to.be.a('string');
    expect(signature).to.be.a('string');
    req.reply({
      statusCode: 200,
      body: {
        challenge: 'challengeString',
      },
    });
  }).as('confirmChallenge');

  cy.intercept('PUT', `/api/booking/1/pick-up`, (req) => {
    req.reply({
      statusCode: 200,
      body: {
        id: 1,
        bookerEmail: bookerEmail,
        bookingDate: bookingDate,
        confirmationDate: confirmationDate,
        closureDate: new Date(2026, 11, 12, 12, 0, 0),
        publicKey: publicKey,
        status: 'PickedUp',
        details: details,
      },
    });
  }).as('pickUpBooking');

  cy.get('button[id="pick-up-btn"]').should('be.visible').click();
});

Then('se verifica que la reserva es mia', () => {
  cy.wait('@postChallenge');
  cy.wait('@confirmChallenge');
});

And('la reserva cambia a estado {string}', (status) => {
  cy.wait('@pickUpBooking').then(({ response }) => {
    expect(response!.body.status).to.equal(status);
    reusableResponse = response!.body;
  });
});

And('la fecha de cierre se actualiza al "12.12.2026" a las "12:00"', () => {
  expect(reusableResponse).to.exist;
  expect(reusableResponse.closureDate).to.equal(new Date(2026, 11, 12, 12, 0, 0).toISOString());
});

And('intento retirar la reserva sin éxito', () => {
  cy.get('button[id="pick-up-btn"]').click();
});

Then('se muestra el mensaje {string}', (message) => {
  cy.get('.customToastBody').should('be.visible').should('contain.text', message);
});

And('la reserva mantiene el estado {string}', (state) => {
  cy.get('p[id="booking-status"]').should('be.visible').should('contain', state);
});

When('presento como clave privada mi clave privada, menos un dígito', () => {
  cy.get('input[id="private-key-input"]').should('be.visible').type(privateKey.slice(0, -1));
});

And('intento retirarla', () => {
  cy.intercept('POST', `/api/booking/1/challenge`, (req) => {
    req.reply({
      statusCode: 201,
      body: {
        challenge: 'challengeString',
      },
    });
  }).as('postChallenge');

  cy.get('button[id="pick-up-btn"]').should('be.visible').click();
});

Then('se verifica que la reserva no es mia', () => {
  cy.wait('@postChallenge');
});

When('presento la clave con la que cuento para la reserva con id "2"', () => {
  cy.get('input[id="private-key-input"]').should('be.visible').type(privateKey);
  cy.intercept('POST', `/api/booking/1/challenge-confirm`, (req) => {
    const { challenge, signature } = req.body;
    expect(challenge).to.be.a('string');
    expect(signature).to.be.a('string');
    req.reply({
      statusCode: 400,
      body: {
        message: 'The provided private key is not valid for this reservation.',
      },
    });
  }).as('confirmChallenge');
});

Then('se verifica que la reserva no corresponde a la clave', () => {
  cy.wait('@postChallenge');
  cy.wait('@confirmChallenge');
});
