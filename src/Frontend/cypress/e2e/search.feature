Feature: hacer busquedas de medicamentos

  Scenario: Botón Clear search restaura lista y limpia input
    Given que hay una lista inicial de medicamentos con "Perifar" y "Kalitron"
    And busco por "Perifar"
    And solo se muestra "Perifar"
    When presiono el boton "Clear search"
    Then se muestran "Perifar" y "Kalitron"
    And el input de busqueda queda vacio