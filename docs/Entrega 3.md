# Planificación y organización iniciales
#### Definición de roles
- **Desarrollo y test:** Victoria Topolansky
- **Product owner:** Shazira Giosa
- **Coach Kanban:** Matías Ohanian

#### Plan de la entrega
Para esta entrega, primeramente vamos a definir los escenarios y criterios de aceptación en BDD de las nuevas funcionalidades en colaboración con el PO y el resto del equipo. Luego, verificaremos los procesos de gestión de ingeniería para adaptarlos a la aplicación de BDD. 
Posteriormente, se implementarán las funcionalidades definidas a través de BDD y TDD según corresponda. Finalizaremos realizando los diagramas de vistas 4+1 para las funcionalidades nuevas y diagrama de componentes para toda la solucion. 

# Adaptación de los procesos de gestión e ingeniería para aplicar BDD 

![Pipeline automatizado actualizado](/docs/img/pipeline-automatizacion-actualizado.jpg)
Podemos ver la versión actualizada del pipeline de despliegue en la imagen.
Para integrar las nuevas pruebas del frontend decidimos extender el workflow que se utiliza para el build del frontend. En una primera instancia pensamos en realizar job independiente que contuviera la lógica para la action de los tests para una clara separación de responsabilidades. Sin embargo, cada job se ejecuta en un entorno aislado, esto implica la reinstalación de las dependencias (npm install) o incorporar formas de transferencias entre jobs, ambas cosas aumentan el tiempo total del pipeline. Por ende, se decidió dejar todo en el mismo job y llamarlo “Build and test”.


# User Stories, criterios de aceptación y escenarios de prueba BDD

![Example mapping crear reserva](/docs/img/Example-Mapping-Crear-reserva.jpg)
![Example mapping listar reserva](/docs/img/Example-Mapping-Listar-reserva.jpg)
![Example mapping confirmar reserva](/docs/img/Example-Mapping-Confirmar-reserva.jpg)
![Example mapping cancelar reserva](/docs/img/Example-Mapping-Cancelar-reserva.jpg)
![Example mapping retirar reserva](/docs/img/Example-Mapping-Retirar-reserva.jpg)
![Example mapping subir receta en reserva](/docs/img/Example-Mapping-Subir-receta-reserva.jpg)


# Evidencia de implementación de nuevas funcionalidades con TDD y BDD

# Diagramas 
## Vista de procesos 4+1 (diagramas de secuencia)
![Listar reservas diagrama de secuencia](/docs/img/listar-resrvas-diagrama-secuencia.png)
![Aceptar reserva diagrama de secuencia](/docs/img/aceptar-reserva-digrama-secuencia.png)

## Diagrama de componentes
![Diagrama de componentes de toda la solución](/docs/img/diagrama-componentes.jpg)

