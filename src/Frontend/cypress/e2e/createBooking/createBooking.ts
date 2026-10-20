import { Given, And, When, Then } from 'cypress-cucumber-preprocessor/steps';

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

And('me encuentro mirando el medicamento {string}', (name) => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'PERIFAR',
      name: name,
      symptom: 'headache',
      stock: 10,
      price: 50,
      quantity: 1,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  }).as('getDrug');
  cy.visit('http://localhost:4200/home/detail/1');
  cy.wait('@getDrug');
});

And('selecciono que quiero reservar {string}', (quantity) => {
  cy.get('#quantityInput').should('have.value', parseInt(quantity));
});

When('presiono el boton "Add to booking" de color {string}', (color) => {
  cy.get('button[id="addToBooking"]').should('have.css', 'background-color', color).click();
});

Then('se agrega {string} con "1" unidad a mi carrito de reservas', (name) => {
  cy.window().then((win) => {
    const booking = win.localStorage.getItem('booking') || '[]';
    expect(booking).to.deep.equal(
      JSON.stringify([
        {
          id: 1,
          name: name,
          code: 'PERIFAR',
          price: 50,
          quantity: 1,
          prescription: false,
          pharmacy: {
            id: 1,
            name: 'Pharmacy Ala',
          },
        },
      ]),
    );
  });
});

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

Then('ya inicie una reserva con la farmacia {string}', () => {
  cy.window().then((win) => {
    win.localStorage.setItem(
      'booking',
      JSON.stringify([
        {
          id: 1,
          name: 'Perifar',
          price: 50,
          quantity: 1,
          prescription: false,
          pharmacy: {
            id: 1,
            name: 'Pharmacy Ala',
          },
        },
      ]),
    );
  });
});

When('quiero agregar un medicamento de la farmacia {string} a la reserva', (pharmacy) => {
  cy.intercept('GET', '/api/drug/2', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'PERIFAR',
      name: 'Perifar',
      symptom: 'headache',
      stock: 10,
      price: 50,
      quantity: 1,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 2,
        name: pharmacy,
      },
    },
  }).as('getDrug');
  cy.visit('http://localhost:4200/home/detail/2');
  cy.wait('@getDrug');
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] });
  cy.get('button[id="addToBooking"]').click();
});

Then('no se agrega el medicamento a la reserva', () => {
  cy.window().then((win) => {
    const booking = win.localStorage.getItem('booking') || '[]';
    expect(booking).to.deep.equal(
      JSON.stringify([
        {
          id: 1,
          name: 'Perifar',
          price: 50,
          quantity: 1,
          prescription: false,
          pharmacy: {
            id: 1,
            name: 'Pharmacy Ala',
          },
        },
      ]),
    );
  });
});

And('se muestra el mensaje {string}', (message) => {
  cy.get('.customToastBody').should('be.visible').should('contain.text', message);
});

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

When('quiero hacer una reserva de {string}', (name) => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'PERIFAR',
      name: name,
      symptom: 'headache',
      stock: 10,
      price: 50,
      quantity: 1,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  }).as('getDrug');
  cy.visit('http://localhost:4200/home/detail/1');
  cy.wait('@getDrug');
});

And('selecciono {string} unidades', (quantity) => {
  cy.get('#quantityInput').clear().type(quantity);
  cy.get('button[id="addToBooking"]').click();
});

Then('el medicamento no es agregado al carrito de reservas', () => {
  cy.window().then((win) => {
    const booking = win.localStorage.getItem('booking') || '[]';
    expect(booking).to.deep.equal(JSON.stringify([]));
  });
});

Given('que soy un cliente', () => {
  cy.window().then((win) => {
    expect(win.localStorage.length).to.equal(0);
  });
});

And('hay {string} existencia de {string}', (quantity, name) => {
  cy.intercept('GET', '/api/drug/1', {
    statusCode: 200,
    body: {
      id: 1,
      code: 'PERIFAR',
      name: name,
      symptom: 'headache',
      stock: parseInt(quantity),
      price: 50,
      quantity: 1,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  }).as('getDrug');
});

And('{string} existencia de {string}', (quantity, name) => {
  cy.intercept('GET', '/api/drug/2', {
    statusCode: 200,
    body: {
      id: 2,
      code: 'KALITRON',
      name: name,
      symptom: 'headache',
      stock: parseInt(quantity),
      price: 50,
      quantity: 1,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  }).as('getDrug');
});

When('quiero reservar {string} unidades de {string}', (quantity) => {
  cy.visit('http://localhost:4200/home/detail/1');
  cy.wait('@getDrug');
  cy.get('#quantityInput').clear().type(quantity);
  cy.get('button[id="addToBooking"]').click();
});

And('{string} unidad de {string}', (quantity) => {
  cy.visit('http://localhost:4200/home/detail/2');
  cy.wait('@getDrug');
  cy.get('#quantityInput').clear().type(quantity);
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] });
  cy.get('button[id="addToBooking"]').click();

  cy.intercept(
    {
      method: 'GET',
      url: '**/api/drug*',
    },
    (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            id: 1,
            code: 'PERIFAR',
            name: 'Perifar',
            symptom: 'headache',
            stock: parseInt(quantity),
            price: 50,
            quantity: 1,
            prescription: false,
            unitMeasure: 'tablet',
            presentation: 'box',
            pharmacy: {
              id: 1,
              name: 'Pharmacy Ala',
            },
          },
          {
            id: 2,
            code: 'KALITRON',
            name: 'Kalitron',
            symptom: 'headache',
            stock: parseInt(quantity),
            price: 50,
            quantity: 1,
            prescription: false,
            unitMeasure: 'tablet',
            presentation: 'box',
            pharmacy: {
              id: 1,
              name: 'Pharmacy Ala',
            },
          },
        ],
      });
    },
  ).as('getDrugsFilter');

  cy.get('app-booking-cart').find('.customCheckout').click();
  cy.wait('@getDrugsFilter');
});

Then('la reserva no es creada', () => {
  cy.window().url().should('include', '/home/cart');
});

And('estamos en la fecha 12.12.2026 a las 12:00', () => {
  const fixedDate = new Date(2026, 11, 12, 12, 0, 0);
  cy.clock(fixedDate.getTime());
});

When(
  'quiero hacer una reserva indicando un medicamento, mi correo, la cantidad del medicamento y la receta necesaria',
  () => {
    const bookingCart = [
      {
        id: 3,
        code: 'TRAMADOL',
        name: 'Tramadol',
        symptom: 'severe pain',
        stock: 1,
        price: 300,
        quantity: 1,
        prescription: true,
        unitMeasure: 'tablet',
        presentation: 'box',
        pharmacy: {
          id: 1,
          name: 'Pharmacy Ala',
        },
      },
    ];

    cy.window().then((win) => {
      win.localStorage.setItem('booking', JSON.stringify(bookingCart));
    });

    cy.visit('http://localhost:4200/home/cart');

    cy.get('#addPrescriptionBtnInfo').click();
    cy.get('input[type="file"]').should('exist');

    cy.get('input[type="file"]').selectFile(`cypress/fixtures/prescription.jpg`, {
      force: true,
    });

    cy.intercept(
      {
        method: 'GET',
        url: '**/api/drug*',
      },
      (req) => {
        req.reply({
          statusCode: 200,
          body: [
            {
              id: 3,
              code: 'TRAMADOL',
              name: 'Tramadol',
              symptom: 'severe pain',
              stock: 1,
              price: 300,
              quantity: 1,
              prescription: true,
              unitMeasure: 'tablet',
              presentation: 'box',
              pharmacy: {
                id: 1,
                name: 'Pharmacy Ala',
              },
            },
          ],
        });
      },
    ).as('getDrugsFilter');

    cy.get('app-booking-cart').find('.customCheckout').click();
    cy.wait('@getDrugsFilter');
    cy.window().then((win) => {
      const booking = win.localStorage.getItem('booking');
      expect(JSON.parse(booking!)[0].prescriptionImage).to.include('data:image/jpeg;base64,');
    });

    cy.window().url().should('include', '/home/booking-cart/checkout');
    cy.get('#EmailInput').type('user@example.com');
  },
);

Then('la reserva es creada con los datos indicados', () => {
  cy.intercept('POST', '/api/booking', (req) => {
    expect(req.body.bookerEmail).to.be.equal('user@example.com');
    const detail = req.body.details[0];
    expect(detail.code).to.equal('TRAMADOL');
    expect(detail.quantity).to.equal(1);
    expect(detail.pharmacyId).to.equal(1);
    expect(detail.prescriptionImage).to.include('data:image/jpeg;base64,');

    req.reply({
      statusCode: 201,
      body: {
        id: 1,
        bookerEmail: req.body.bookerEmail,
        bookingDate: req.body.bookingDate,
        confirmationDate: '',
        closureDate: '',
        publicKey: req.body.publicKey,
        status: 'Pending',
      },
    });
  }).as('createBooking');

  cy.get('#finishBookingButton').click();
});

And(
  'se asigna fecha de inicio 12.12.2026 a las 12:00, fecha de confirmacion -, id, clave publica, fecha de cierre - y estado Pending',
  () => {
    cy.wait('@createBooking').then(({ request, response }) => {
      expect(response!.body.bookingDate).to.equal(request.body.bookingDate);
      expect(response!.body.publicKey).to.equal(request.body.publicKey);
      expect(response!.body.id).to.equal(1);
      expect(response!.body.confirmationDate).to.be.empty;
      expect(response!.body.closureDate).to.be.empty;
      expect(response!.body.status).to.equal('Pending');
    });
  },
);

And('hay existencias de {string}', (name) => {
  cy.intercept('GET', '/api/drug/3', {
    statusCode: 200,
    body: {
      id: 3,
      code: 'TRAMADOL',
      name: name,
      symptom: 'headache',
      stock: 1,
      price: 300,
      quantity: 1,
      prescription: true,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  }).as('getDrug');
});

When('quiero reservar {string} unidad de Tramadol', (quantity) => {
  cy.visit('http://localhost:4200/home/detail/3');
  cy.wait('@getDrug');
  cy.get('#quantityInput').clear().type(quantity);
  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] });
  cy.get('button[id="addToBooking"]').click();
});

Then('la reserva es creada con estado "Pending Validation"', () => {
  cy.intercept(
    {
      method: 'GET',
      url: '**/api/drug*',
    },
    (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            id: 3,
            code: 'TRAMADOL',
            name: 'Tramadol',
            symptom: 'severe pain',
            stock: 1,
            price: 300,
            quantity: 1,
            prescription: true,
            unitMeasure: 'tablet',
            presentation: 'box',
            pharmacy: {
              id: 1,
              name: 'Pharmacy Ala',
            },
          },
        ],
      });
    },
  ).as('getDrugsFilter');

  cy.get('app-booking-cart').find('.customCheckout').click();
  cy.wait('@getDrugsFilter');

  cy.window().url().should('include', '/home/booking-cart/checkout');
  cy.get('#EmailInput').type('user@example.com');

  cy.intercept('POST', '/api/booking', (req) => {
    expect(req.body.bookerEmail).to.be.equal('user@example.com');
    expect(req.body.hasPrescriptionAll).to.be.false;
    const detail = req.body.details[0];
    expect(detail.code).to.equal('TRAMADOL');
    expect(detail.quantity).to.equal(1);
    expect(detail.pharmacyId).to.equal(1);

    req.reply({
      statusCode: 201,
      body: {
        id: 1,
        bookerEmail: req.body.bookerEmail,
        bookingDate: req.body.bookingDate,
        confirmationDate: '',
        closureDate: '',
        publicKey: req.body.publicKey,
        status: 'Pending Validation',
      },
    });
  }).as('createBooking');

  cy.get('#finishBookingButton').click();
});

When('creo una reserva', () => {
  const bookingCart = [
    {
      id: 1,
      code: 'PERIFAR',
      name: 'Perifar',
      symptom: 'headache',
      stock: 10,
      price: 50,
      quantity: 2,
      prescription: false,
      unitMeasure: 'tablet',
      presentation: 'box',
      pharmacy: {
        id: 1,
        name: 'Pharmacy Ala',
      },
    },
  ];
  cy.window().then((win) => {
    win.localStorage.setItem('booking', JSON.stringify(bookingCart));
    win.localStorage.setItem('totalbooking', JSON.stringify(100));
  });

  cy.intercept('GET', '**/api/pharmacy**', { statusCode: 200, body: [] }).as('getPharmacies');
  cy.intercept('GET', '**/api/drug**', { statusCode: 200, body: [] }).as('getDrugs');
  cy.visit('http://localhost:4200/home/booking-cart/checkout');
  cy.wait('@getPharmacies');

  cy.get('#EmailInput').type('user@example.com');
  cy.intercept('POST', '/api/booking', (req) => {
    req.reply({
      statusCode: 201,
      body: {
        id: 1,
        bookerEmail: req.body.bookerEmail,
        bookingDate: req.body.bookingDate,
        confirmationDate: '',
        closureDate: '',
        publicKey: req.body.publicKey,
        status: 'Pending',
      },
    });
  }).as('createBooking');

  cy.get('#finishBookingButton').click();
  cy.wait('@getDrugs');
  cy.wait('@createBooking');
});

Then('veo un mensaje que dice {string}', (message) => {
  cy.get('.customToastBody').should('be.visible').should('contain.text', message);
});

And('mi clave privada junto al id de la reserva', () => {
  cy.get('.customToastBody')
    .should('be.visible')
    .invoke('text')
    .then((text) => {
      const idMatch = text.match(/Id: (\d+)/);
      expect(idMatch).to.not.be.null;
      const id = idMatch![1];
      expect(id).to.match(/^\d+$/);
      expect(parseInt(id)).to.be.greaterThan(0);
    });

  cy.contains(/Private Key: \S+/)
    .invoke('text')
    .then((text) => {
      const key = text.split('Private Key: ')[1];
      expect(key).to.match(/^[A-Za-z0-9+/]+=*$/);
      expect(key).to.have.length(88);
    });
});
