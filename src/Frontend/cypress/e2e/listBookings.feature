Feature: Listar reservas de medicamentos

    Scenario: listar medicamentos sin una reserva hecha
        Given que soy un cliente
        When ingreso mi mail para ver la lista de reservas
        And realicé ninguna reserva aún
        Then no se muestra ninguna lista
        And se ve el mensaje "There are no reservations associated with that email address."

    Scenario: listar medicamentos cuando se ingresa un correo inválido
        Given que soy un cliente
        When ingreso "c" en el campo de correo para la busqueda de reservas
        Then no se muestra ninguna lista
        And se ve el mensaje "The email address entered is invalid."

    Scenario: listar medicamentos con sus datos
        Given que soy un cliente
        When estoy en la pantalla de mi lista de reservas
        And tengo una reserva en la farmacia "Luna" de "2" unidades de "Perifar" y "1" de "Kalitron" creada el "17.5.2026" a las "11:00" con el estado "Pending"
        Then veo mi reserva de la farmacia "Luna" con "2" unidades de "Perifar" y "1" de "Kalitron" con fecha de inicio "17.5.2026" a las "11:00", fecha de confirmacion -, fecha de cierre - y con el estado "Pending"

    Scenario: listar reservas en el orden correctos
        Given que soy un cliente
        When ingreso mi mail para ver la lista de reservas
        And tengo 6 reservas, cada una con uno de los siguientes estados: "Pending", "Pending Validation", "Confirmed", "Expired", "Canceled", y "Picked up"
        Then veo las reservas, por su estado, en el siguiente orden: "Pending Validation", "Confirmed", "Pending", "Canceled", "Expired" y "Picked up"
