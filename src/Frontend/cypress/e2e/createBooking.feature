Feature: Crear reserva de medicamentos

    Scenario: Agregar un medicamento a una reserva
        Given que soy un cliente
        And me encuentro mirando el medicamento "Perifar"
        And selecciono que quiero reservar "1"
        When presiono el boton "Add to booking" de color "rgb(255, 103, 15)"
        Then se agrega "Perifar" con "1" unidad a mi carrito de reservas

    Scenario: Reservar un medicamento de una farmacia distinta a la elegida
        Given que soy un cliente
        And ya inicie una reserva con la farmacia "Sol"
        When quiero agregar un medicamento de la farmacia "Luna" a la reserva
        Then no se agrega el medicamento a la reserva
        And se muestra el mensaje "All medications in the reservation must belong to the same pharmacy."

    Scenario: Reserva con cantidad 0
        Given que soy un cliente
        When quiero hacer una reserva de "Perifar"
        And selecciono "0" unidades
        Then el medicamento no es agregado al carrito de reservas
        And se muestra el mensaje "Product count cant be 0"

    Scenario: Reservar medicamento sin stock de uno de los medicamentos
        Given que soy un cliente
        And hay "1" existencia de "Perifar"
        And "1" existencia de "Kalitron"
        When quiero reservar "2" unidades de "Perifar"
        And "1" unidad de "Kalitron"
        Then la reserva no es creada
        And se muestra el mensaje "Insuficient stock"

    Scenario: Reserva con datos correctos
        Given que soy un cliente
        And estamos en la fecha 12.12.2026 a las 12:00
        When quiero hacer una reserva indicando un medicamento, mi correo, la cantidad del medicamento y la receta necesaria
        Then la reserva es creada con los datos indicados
        And se asigna fecha de inicio 12.12.2026 a las 12:00, fecha de confirmacion -, id, clave publica, fecha de cierre - y estado Pending

    Scenario: Reservar medicamento con stock y con necesidad de receta
        Given que soy un cliente
        And hay existencias de "Tramadol"
        When quiero reservar "1" unidad de Tramadol
        Then la reserva es creada con estado "Pending Validation"

    Scenario: Obtener clave privada de una reserva
        Given que soy un cliente
        When creo una reserva
        Then veo un mensaje que dice "Booking created successfully"
        And mi clave privada junto al id de la reserva