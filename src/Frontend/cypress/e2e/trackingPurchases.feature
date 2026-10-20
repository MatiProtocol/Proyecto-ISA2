Feature: Tracking de compras por códigos

  Scenario: No agregar código vacío a recientes
    Given historial de tracking vacío
    When presiono el boton "Track your purchase" con el input vacío
    Then no se muestra la sección "Last 5 tracked codes"

  Scenario: No agregar código inválido a recientes
    Given historial de tracking con "YJ1VVFCTBBC6J1JX"
    And la consulta por código "LP4ZVWSABEC0JPJ7" retorna sin resultado
    When ingreso "LP4ZVWSABEC0JPJ7" y presiono el boton "Track your purchase"
    Then la tabla de recientes contiene solo "YJ1VVFCTBBC6J1JX"