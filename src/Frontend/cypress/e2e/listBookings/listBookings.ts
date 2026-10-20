import { Given, And, When, Then } from 'cypress-cucumber-preprocessor/steps';

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

When('ingreso mi mail para ver la lista de reservas', () => {
  cy.intercept('GET', '**/api/pharmacy**', {
    statusCode: 200,
    body: [],
  });

  cy.visit('http://localhost:4200/home/bookings');
  cy.get('#bookingsEmailInput').type('user@example.com');
});

And('realicé ninguna reserva aún', () => {
  cy.intercept('GET', '/api/booking?*', {
    statusCode: 200,
    body: [],
  }).as('getBookings');

  cy.get('button[id="listBookingsButton"]').click();
  cy.wait('@getBookings').then(({ response }) => {
    expect(response!.body).to.be.empty;
  });
});

Then('no se muestra ninguna lista', () => {
  cy.get('app-bookings-list').should('not.exist');
  cy.get('.bookingsList').should('not.exist');
});

And('se ve el mensaje {string}', (message) => {
  cy.get('.customToastBody').should('be.visible').should('contain.text', message);
});

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

When('ingreso {string} en el campo de correo para la busqueda de reservas', (invalidMail) => {
  cy.intercept('GET', '/api/booking?*', {
    statusCode: 400,
    body: { message: 'Invalid Email' },
  }).as('getBookingsError');

  cy.intercept('GET', '**/api/pharmacy**', {
    statusCode: 200,
    body: [],
  });

  cy.visit('http://localhost:4200/home/bookings');
  cy.get('#bookingsEmailInput').type(invalidMail);
  cy.get('button[id="listBookingsButton"]').click();
  cy.wait('@getBookingsError');
});

Then('no se muestra ninguna lista', () => {
  cy.get('app-bookings-list').should('not.exist');
  cy.get('.bookingsList').should('not.exist');
});

And('se ve el mensaje {string}', (message) => {
  cy.get('.customToastBody').should('be.visible').should('contain.text', message);
});

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

When('estoy en la pantalla de mi lista de reservas', () => {
  cy.intercept('GET', '**/api/pharmacy**', {
    statusCode: 200,
    body: [],
  });

  cy.visit('http://localhost:4200/home/bookings');
  cy.get('#bookingsEmailInput').type('user@example.com');
});

And(
  'tengo una reserva en la farmacia {string} de {string} unidades de {string} y {string} de {string} creada el {string} a las {string} con el estado {string}',
  (
    pharmacy,
    firstMedicationUnits,
    firstMedication,
    secondMedicationUnits,
    secondMedication,
    date,
    time,
    status,
  ) => {
    const formattedDate = date.replace(/\./g, '/');
    const expectedBookingDate = `${formattedDate} ${time}`;

    cy.intercept('GET', '/api/booking?*', {
      statusCode: 200,
      body: [
        {
          id: 1,
          bookerEmail: 'user@example.com',
          bookingDate: expectedBookingDate,
          confirmationDate: '',
          closureDate: '',
          status: status,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: firstMedication.toUpperCase(),
              name: firstMedication,
              quantity: parseInt(firstMedicationUnits),
              price: 0,
              pharmacyId: 1,
              pharmacyName: pharmacy,
            },
            {
              id: 2,
              code: secondMedication.toUpperCase(),
              name: secondMedication,
              quantity: parseInt(secondMedicationUnits),
              price: 0,
              pharmacyId: 1,
              pharmacyName: pharmacy,
            },
          ],
        },
      ],
    }).as('getBookings');
    cy.get('button[id="listBookingsButton"]').click();

    cy.wait('@getBookings');
  },
);

Then(
  'veo mi reserva de la farmacia {string} con {string} unidades de {string} y {string} de {string} con fecha de inicio {string} a las {string}, fecha de confirmacion -, fecha de cierre - y con el estado {string}',
  (
    pharmacy,
    firstMedicationUnits,
    firstMedication,
    secondMedicationUnits,
    secondMedication,
    bookingDate,
    bookingTime,
    status,
  ) => {
    const formattedDate = bookingDate.replace(/\./g, '/');
    const expectedBookingDate = `${formattedDate} ${bookingTime}`;

    cy.get('c-card')
      .first()
      .within(() => {
        cy.get('p').should('contain', `${expectedBookingDate}`);
        cy.get('p').should('contain', status);

        cy.get('table').within(() => {
          cy.get('tbody tr').should('have.length', 2);

          cy.get('tbody tr')
            .eq(0)
            .within(() => {
              cy.get('td').eq(0).should('contain', firstMedication);
              cy.get('td').eq(1).should('contain', firstMedicationUnits);
              cy.get('td').eq(2).should('contain', pharmacy);
            });

          cy.get('tbody tr')
            .eq(1)
            .within(() => {
              cy.get('td').eq(0).should('contain', secondMedication);
              cy.get('td').eq(1).should('contain', secondMedicationUnits);
              cy.get('td').eq(2).should('contain', pharmacy);
            });
        });
      });
  },
);

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

When('ingreso mi mail para ver la lista de reservas', () => {
  cy.intercept('GET', '**/api/pharmacy**', {
    statusCode: 200,
    body: [],
  });

  cy.visit('http://localhost:4200/home/bookings');
  cy.get('#bookingsEmailInput').type('user@example.com');
});

And(
  'tengo 6 reservas, cada una con uno de los siguientes estados: {string}, {string}, {string}, {string}, {string}, y {string}',
  (
    pendingState,
    pendingValidationState,
    confirmedState,
    expiredState,
    canceledState,
    pickedUpState,
  ) => {
    cy.intercept('GET', '**/api/booking?*', {
      statusCode: 200,
      body: [
        {
          id: 1,
          bookerEmail: 'user@example.com',
          bookingDate: '17/5/2026 11:00',
          confirmationDate: '',
          closureDate: '',
          status: pendingState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'PERIFAR',
              name: 'Perifar',
              quantity: 1,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
            {
              id: 2,
              code: 'KALITRON',
              name: 'Kalitron',
              quantity: 1,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
        {
          id: 2,
          bookerEmail: 'user@example.com',
          bookingDate: '17/5/2026 11:00',
          confirmationDate: '',
          closureDate: '',
          status: pendingValidationState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'TRAMADOL',
              name: 'Tramadol',
              quantity: 1,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
        {
          id: 3,
          bookerEmail: 'user@example.com',
          bookingDate: '18/5/2026 11:00',
          confirmationDate: '18/5/2026 11:00',
          closureDate: '',
          status: confirmedState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'KALITRON',
              name: 'Kalitron',
              quantity: 1,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
        {
          id: 4,
          bookerEmail: 'user@example.com',
          bookingDate: '03/5/2026 11:00',
          confirmationDate: '03/5/2026 19:00',
          closureDate: '05/5/2026 19:00',
          status: expiredState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'KALITRON',
              name: 'Kalitron',
              quantity: 1,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
        {
          id: 5,
          bookerEmail: 'user@example.com',
          bookingDate: '07/4/2026 11:00',
          confirmationDate: '',
          closureDate: '07/4/2026 14:00',
          status: canceledState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'PERIFAR',
              name: 'Perifar',
              quantity: 7,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
        {
          id: 6,
          bookerEmail: 'user@example.com',
          bookingDate: '07/4/2026 11:00',
          confirmationDate: '07/4/2026 22:00',
          closureDate: '08/4/2026 14:00',
          status: pickedUpState,
          totalAmount: 0,
          publicKey: '',
          details: [
            {
              id: 1,
              code: 'Kalitron',
              name: 'Kalitron',
              quantity: 2,
              price: 0,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
      ],
    }).as('getBookings');
    cy.get('button[id="listBookingsButton"]').click();
    cy.wait('@getBookings');
  },
);

Then(
  'veo las reservas, por su estado, en el siguiente orden: {string}, {string}, {string}, {string}, {string} y {string}',
  (firstStatus, secondStatus, thirdStatus, fourthStatus, fifthStatus, sixthStatus) => {
    const expectedStatuses = [
      firstStatus,
      secondStatus,
      thirdStatus,
      fourthStatus,
      fifthStatus,
      sixthStatus,
    ];

    cy.get('c-card').each((card, index) => {
      cy.wrap(card).within(() => {
        cy.get('c-card-body p b').then(($elements) => {
          const status = $elements.eq($elements.length - 1).text();
          expect(status).to.equal(expectedStatuses[index]);
        });
      });
    });
  },
);
