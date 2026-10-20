Feature: retirar reservas

      Scenario: retirar reserva con estado "Confirmed"
          Given que soy un cliente
          And tengo una reserva de id "1" en estado "Confirmed"
          When presento la clave privada para la reserva de id "1"
          And estamos a "12.12.2026" a las "12:00"
          And quiero retirarla
          Then se verifica que la reserva es mia
          And la reserva cambia a estado "PickedUp"
          And la fecha de cierre se actualiza al "12.12.2026" a las "12:00"

      Scenario Outline: retirar reserva con estado distinto a "Confirmed"
          Given que soy un cliente
          And tengo una reserva de id "1" en estado <state>
          When presento la clave privada para la reserva de id "1"
          And intento retirarla
          Then se muestra el mensaje "Only confirmed reservations can be withdrawn."
          And la reserva mantiene el estado <state>

          Examples:
        | state                 |
        | "Pending"             |
        | "PendingValidation"   |
        | "Expired"             |
        | "Canceled"            |
        | "PickedUp"            |

Scenario: firmar con clave privada incorrecta
          Given que soy un cliente
          And tengo una reserva de id "1" en estado "Confirmed"
          When presento como clave privada mi clave privada, menos un dígito
          And intento retirarla
          Then se verifica que la reserva no es mia
          And se muestra el mensaje "The provided private key is not valid for this reservation."

Scenario: firmar con clave privada incorrecta para id
          Given que soy un cliente
          And tengo una reserva de id "1" en estado "Confirmed"
          When presento la clave con la que cuento para la reserva con id "2"
          And intento retirarla
          Then se verifica que la reserva no corresponde a la clave
          And se muestra el mensaje "The provided private key is not valid for this reservation."
