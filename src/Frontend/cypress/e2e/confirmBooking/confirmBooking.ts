import { Given, And, When, Then } from 'cypress-cucumber-preprocessor/steps';

const dateToday = new Date().toISOString().split('T');
const dateFormat =
  dateToday[0] + ' ' + dateToday[1].split(':')[0] + ':' + dateToday[1].split(':')[1];

Given('que soy un empleado de la farmacia "Luna"', () => {
  cy.visit('http://localhost:4200/login');
  cy.get('input[placeholder="Username"]').type('maria.gomez');
  cy.get('input[placeholder="Password"]').type('A123456.');
  cy.intercept('POST', 'http://localhost:5000/api/login').as('loginRequest');

  cy.intercept('POST', '/api/login', (req) => {
    expect(req.body.userName).to.be.equal('maria.gomez');
    expect(req.body.password).to.be.equal('A123456.');

    req.reply({
      statusCode: 200,
      body: {
        token: '22222222-2222-2222-2222-222222222222',
        role: 'Employee',
        userName: 'maria.gomez',
      },
    });
  }).as('loginRequest');

  cy.get('button[id="loginButton"]').click();
  cy.wait('@loginRequest');

  cy.url().should('include', '/employee');

  cy.get('.customToastBody')
    .should('be.visible')
    .should('contain.text', 'Login successful. Welcome: maria.gomez');
});

And('que hay una reserva de 2 unidades de Perifar a la farmacia "Luna"', () => {
  cy.intercept('GET', 'http://localhost:5000/api/booking/by-pharmacy', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id: 1,
          status: 'Pending',
          bookingDate: dateFormat,
          details: [
            {
              code: 'P001',
              name: 'Perifar',
              quantity: 2,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
      ],
    });
  }).as('getBookings');
  cy.get('a[routerlink="/employee/booking-requests"]').click();
  cy.wait('@getBookings');
});

And('la farmacia cuenta con un stock de 2 unidades de Perifar', () => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'P001',
      name: 'Perifar',
      symptom: 'headache',
      quantity: 10,
      stock: 2,
      price: 50,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Luna',
      },
    },
  }).as('getDrugInitial');
});

When('intento confirmar la reserva', () => {
  cy.intercept('POST', '/api/booking/1/approve', (req) => {
    expect(req.body.bookingId).to.be.equal(1);
    expect(req.body.pharmacyId).to.be.equal(1);
    expect(req.body.approverName).to.be.equal('maria.gomez');

    req.reply({
      statusCode: 200,
      body: { success: true },
    });
  }).as('approveBooking');

  cy.get('button').contains('Approve').click();
  cy.wait('@approveBooking');
});

Then('la reserva es confirmada', () => {
  cy.contains('Confirmed').should('exist');
});

And('pasa al estado "Confirmed"', () => {
  cy.contains('Confirmed').should('exist');
});

And('el stock para Perifar en la farmacia "Luna" pasa a 0', () => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'P001',
      name: 'Perifar',
      symptom: 'headache',
      quantity: 0,
      stock: 0,
      price: 50,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Luna',
      },
    },
  }).as('getDrugZero');

  cy.visit('http://localhost:4200/home/detail/1');
  cy.wait('@getDrugZero');

  cy.get('#quantityInput').should('have.attr', 'max', '0');
});

And('que hay una reserva de 1 unidad de Tramadol a la farmacia "Luna"', () => {
  cy.intercept('GET', 'http://localhost:5000/api/booking/by-pharmacy', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id: 10,
          status: 'Pending',
          bookingDate: dateFormat,
          details: [
            {
              code: 'TRAMADOL',
              name: 'Tramadol',
              quantity: 1,
              pharmacyId: 1,
              pharmacyName: 'Luna',
              prescriptionImage: 'data:image/png;base64,unaImagen',
            },
          ],
        },
      ],
    });
  }).as('getBookingsTramadol');

  cy.get('a[routerlink="/employee/booking-requests"]').click();
  cy.wait('@getBookingsTramadol');
});

And('la farmacia cuenta con un stock de 1 unidad de Tramadol', () => {
  cy.intercept('GET', '/api/drug/3', {
    statusCode: 200,
    body: {
      id: 3,
      code: 'TRAMADOL',
      name: 'Tramadol',
      symptom: 'severe pain',
      quantity: 1,
      stock: 1,
      price: 300,
      prescription: true,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Luna',
      },
    },
  }).as('getTramadol');
});

And('la receta adjunta es inválida', () => {
  cy.intercept('POST', '/api/booking/1/cancel', (req) => {
    expect(req.body.bookingId).to.be.equal(10);
    expect(req.body.pharmacyId).to.be.equal(1);
    expect(req.body.approverName).to.exist;

    req.reply({
      statusCode: 200,
      body: { success: true, message: 'Invalid prescription' },
    });
  }).as('cancelBookingInvalid');
});

When('el empleado verifica la receta de la reserva', () => {
  cy.get('button').contains('Cancel').click();
  cy.wait('@cancelBookingInvalid');
});

Then('la reserva es cancelada', () => {
  cy.contains('Canceled').should('exist');
});

And('pasa al estado "Canceled"', () => {
  cy.contains('Canceled').should('exist');
});

And('hay una reserva de 2 Perifar', () => {
  cy.intercept('GET', 'http://localhost:5000/api/booking/by-pharmacy', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id: 1,
          status: 'Pending',
          bookingDate: dateFormat,
          details: [
            {
              code: 'P001',
              name: 'Perifar',
              quantity: 2,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
      ],
    });
  }).as('getBookings');
  cy.get('a[routerlink="/employee/booking-requests"]').click();
  cy.wait('@getBookings');
});

And('se encuentra en estado "Pending"', () => {
  cy.contains('Pending').should('exist');
});

When('hago la verificación de la reserva', () => {
  cy.get('button').contains('Approve');
});

And('noto que el stock de Perifar es 1', () => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'P001',
      name: 'Perifar',
      symptom: 'headache',
      quantity: 1,
      stock: 1,
      price: 50,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Luna',
      },
    },
  }).as('getDrugOne');
  cy.visit('http://localhost:4200/home/detail/1');
  cy.wait('@getDrugOne');
  cy.get('#quantityInput').should('have.attr', 'max', '1');
});

Then('cancelo la reserva', () => {
  cy.visit('http://localhost:4200/employee/booking-requests');

  cy.intercept('POST', '/api/booking/1/cancel', (req) => {
    expect(req.body.bookingId).to.be.equal(1);
    expect(req.body.pharmacyId).to.be.equal(1);
    expect(req.body.approverName).to.exist;
    req.reply({
      statusCode: 200,
      body: { success: true, message: 'Not enough stock' },
    });
  }).as('cancelBookingInvalid');

  cy.get('button').contains('Cancel').click();
  cy.wait('@cancelBookingInvalid');
});

And('el estado es "Canceled"', () => {
  cy.contains('Canceled').should('exist');
});

And('que hay una reserva creada el "12.12.2026" a las "12:00"', () => {
  cy.intercept('GET', 'http://localhost:5000/api/booking/by-pharmacy', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id: 1,
          bookingDate: '2026-12-12T12:00:00Z',
          status: 'Pending',
          details: [
            {
              code: 'P001',
              name: 'Perifar',
              quantity: 2,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
      ],
    });
  }).as('getBookings');
  cy.get('a[routerlink="/employee/booking-requests"]').click();
  cy.wait('@getBookings');
});

And('la fecha actual es "13.12.2026" a las "11:00"', () => {
  const futureDate = new Date('2026-12-13T11:00:00Z').getTime();
  cy.clock(futureDate);
});

And('la farmacia cuenta con un stock suficiente', () => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'P001',
      name: 'Perifar',
      symptom: 'headache',
      quantity: 10,
      stock: 10,
      price: 50,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Luna',
      },
    },
  }).as('getDrug');
});

When('confirmo la reserva', () => {
  cy.intercept('POST', '/api/booking/1/approve', (req) => {
    expect(req.body.bookingId).to.be.equal(1);
    expect(req.body.pharmacyId).to.be.equal(1);
    expect(req.body.approverName).to.exist;
    req.reply({
      statusCode: 200,
      body: { success: true },
    });
  }).as('approveBooking');
  cy.get('button').contains('Approve').click();
  cy.wait('@approveBooking');
});

Then('la reserva es confirmada', () => {
  cy.contains('Confirmed').should('exist');
});

And('la fecha de confirmación para la reserva se establece en "13.12.2026" a las "11:00"', () => {
  cy.intercept('GET', 'http://localhost:5000/api/booking/by-pharmacy', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id: 1,
          bookingDate: '2026-12-12T12:00:00Z',
          confirmationDate: '2026-12-13T11:00:00Z',
          status: 'Confirmed',
          details: [
            {
              code: 'P001',
              name: 'Perifar',
              quantity: 2,
              pharmacyId: 1,
              pharmacyName: 'Luna',
            },
          ],
        },
      ],
    });
  }).as('getBookings');
  cy.visit('http://localhost:4200/employee/booking-requests');
  cy.wait('@getBookings');
  cy.contains('2026-12-13T11:00:00Z').should('exist');
});
