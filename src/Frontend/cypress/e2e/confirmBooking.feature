Feature: Confirmar reservas de medicamentos

    Scenario: Validar reserva con stock suficiente
        Given que soy un empleado de la farmacia "Luna" 
        And que hay una reserva de 2 unidades de Perifar a la farmacia "Luna"
        And la farmacia cuenta con un stock de 2 unidades de Perifar
        When intento confirmar la reserva
        Then la reserva es confirmada
        And pasa al estado "Confirmed"
        And el stock para Perifar en la farmacia "Luna" pasa a 0

    Scenario: Reserva cancelada por receta inválida
        Given que soy un empleado de la farmacia "Luna"
        And que hay una reserva de 1 unidad de Tramadol a la farmacia "Luna"
        And la farmacia cuenta con un stock de 1 unidad de Tramadol
        And la receta adjunta es inválida
        When el empleado verifica la receta de la reserva
        Then la reserva es cancelada
        And pasa al estado "Canceled"

    Scenario: Reserva cancelada por stock insuficiente
        Given que soy un empleado de la farmacia "Luna" 
        And hay una reserva de 2 Perifar
        And se encuentra en estado "Pending"
        When hago la verificación de la reserva
        And noto que el stock de Perifar es 1
        Then cancelo la reserva
        And el estado es "Canceled"

    Scenario: Establecer fecha de confirmación
        Given que soy un empleado de la farmacia "Luna"
        And que hay una reserva creada el "12.12.2026" a las "12:00"
        And la fecha actual es "13.12.2026" a las "11:00"
        And la farmacia cuenta con un stock suficiente
        When confirmo la reserva
        Then la reserva es confirmada
        And la fecha de confirmación para la reserva se establece en "13.12.2026" a las "11:00"