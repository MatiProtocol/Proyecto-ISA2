# Definición del marco de gestión
## Marco de gestión basado en Kanban
### Tablero Kanban
| To Do | Ready to Start | In Progress (WIP=3) | QA Manual | Pending Deployment | In Production |
|:-----:|:--------------:|:-------------------:|:---------:|:------------------:|:-------------:|
|       |                |                     |           |                    |               |

- **To Do:** todas aquellas tareas las cuales aún no se han comenzado ni se encuentran definidas bajo nuestra definición de Ready.
- **Ready To Start:** se encontrarán las tasks provenientes de To Do las cuales ya se han definido bajo nuestra definición de Ready pero no se han comenzado. 
- **In Progress:** aquí las tareas se empiezan a desarrollar con un límite de 3 tareas simultáneas (WIP).
- **QA Manual:** las tareas que estén acá serán aquellas las cuales se debe verificar si se cumplen los requisitos funcionales previo a ser lanzada, enfocado en simular la experiencia de usuario buscando encontrar errores.
- **Pending Deployment:** estarán aquellas tareas las cuales ya se les ha realizado la verificación QA pero aún no se han desplegado a producción.
- **In Production:** siendo el final del tablero, las tareas llegan cuando se encuentran completamente desplegadas en producción.

## Roles
Definimos tres roles los cuales son: 
- **Desarrollo y test:** las personas que ocupen este rol (todo el equipo durante el proyecto) serán las encargadas de escribir el código necesario para el correcto funcionamiento del sistema. Además, crearán tests automáticos y harán tests exploratorios para encontrar las fallas antes de que las funcionalidades lleguen a producción.
- **Product owner:** será la persona encargada de maximizar el valor del producto. Para ello, definirá y priorizará el backlog, detallará las funcionalidades a desarrollar y validará que lo implementado cumpla con los requerimientos. También será responsable de aceptar o rechazar entregables y asegurar que el producto evolucione alineado a los objetivos definidos.
- **Coach Kanban:** la persona que ocupe este rol ayudará a que el equipo haga un buen uso de Kanban. Esto incluye: definir y mantener actualizado el tablero, limitar el trabajo en progreso (WIP), identificar cuellos de botella y promover la mejora continua del proceso. Además, propondrá ajustes en el flujo de trabajo para aumentar la eficiencia. <br>

Se pensó en agregar un tercer rol llamado Code Release Manager que sería encargado de planificar, coordinar y supervisar el ciclo de vida completo del software. Pero fue descartado por el hecho de que en el grupo somos tres personas y mantener el contacto para planificar releases y la evaluación de riesgos es una tarea sencilla.
## Ceremonias
Se definieron las siguientes ceremonias: 
- **Planificación y organización iniciales:** instancia que se da antes de comenzar cada entrega, donde el equipo define y acuerda qué trabajo se abordará a continuación. En esta reunión se revisa el backlog, se priorizan las funcionalidades a desarrollar y se organizan las tareas de forma general, asegurando que todos sepan los objetivos y el alcance de la próxima etapa de trabajo. <br>
- **Reuniones semanales de avance:** en esta instancia, el equipo se reúne una vez a la semana para comentarles a los demás miembros cuáles funcionalidades han terminado o si tienen algún problema que no son capaces de resolver. De esa manera, podemos revisar el estado actual del trabajo, identificar bloqueos, sincronizar tareas y ajustar prioridades si es necesario. Nos permite mantener visibilidad sobre el progreso.
- **Reunión de análisis de procesos y resultados:** instancia de reflexión al final de una entrega. Por un lado, se revisarán los resultados obtenidos (funcionalidades entregadas) y, por otro, se analizará cómo fue el proceso de trabajo. El objetivo es identificar mejoras, reforzar buenas prácticas y definir acciones concretas para optimizar el desempeño del equipo para la siguiente entrega.
##### En cualquier momento: 
- **Reunión de análisis post-mortem de fallos:** se realiza luego de detectar un error relevante en producción o durante el desarrollo. Su objetivo es analizar las causas del fallo, evaluar su impacto y definir acciones concretas para evitar que vuelva a ocurrir, enfocándose en la mejora del proceso.
# Definición del proceso de ingeniería
## Definition of Done
Consideramos que una task se encuentra en Done cuando esta se probó en un entorno similar al de producción y se encuentra lista para ser desplegada. También se espera que las pruebas automatizadas validen código que se espera agregar. Además, se deben cumplir los criterios de aceptación definidos en la task. Por último, se debe haber creado desde el tronco común (rama “develop”).
## Definition of Ready
Cada task para considerarse como tal deberá tener primeramente una prioridad (alta, media, baja o sin prioridad). Luego, tiene que ser estimable el tiempo de realización de dicha tarea (en horas). Buscamos agregar etiquetas sobre dónde impactará mayormente el cambio o implementación (frontend, backend, DB, testing, infra) y otra etiqueta para saber de qué tipo es el cambio (feature, bugfix, improvement). 
Por último, se debe tener bien definido el criterio de aceptación sobre cuándo la task se encuentra contemplada
## Requisitos necesarios para el movimiento de tareas
Más allá del movimiento desde To Do a Ready to Start deben de ser definidos los demás movimientos del tablero.
Tablero de kanban propuesto:
| To Do | Ready to Start | In Progress (WIP=3) | QA Manual | Pending Deployment | In Production |
|:-----:|:--------------:|:-------------------:|:---------:|:------------------:|:-------------:|
|       |                |                     |           |                    |               |

- ***To Do* a *Ready to Start*:** las tareas que se pasan son aquellas que se encuentran definidas totalmente con todos los requisitos para considerarlas *Ready to Start*. 
- ***Ready to Start* a *In Progress*:** las condiciones para que se de este movimiento es que la tarea debe de ser una de las de más alta prioridad que se encuentre en la columna de *Ready to Start*.
- ***In Progress* a *QA Manual*:** para este pasaje será necesario que la tarea se encuentre totalmente implementada con sus pruebas asociadas. Todas éstas pruebas las cuales sean automáticas deben de cumplirse antes de que la tarea se encuentre en la columna *QA Manual*.
- ***QA Manual* a *Pending Deployment*:** una vez que la tarea ha sido testeada manualmente en un entorno similar al de producción de forma satisfactoria pasará a estar pendiente para ser colocada en producción (*Pending Deployment*).
- ***Pending Deployment* a *In Production*:** una vez los cambios son desplegados en producción la tarea es marcada como *In Production*.

## Flujo de desarrollo
Nuestro flujo de desarrollo comenzará con la creación de un issue en GitHub bien definido para, luego, desde nuestra rama tronco principal (rama develop), crear una nueva nombrada con la nomenclatura adecuada. En esta rama nueva se deberá llevar adelante todo el desarrollo donde, también, los commits deben tener la estructura definida más adelante. Una vez terminado el desarrollo, es necesario realizar un Pull Request a develop. Antes de permitir su aceptación, se correrán los tests automáticos configurados (Gated Commits). Si las pruebas mencionadas son exitosas, se deben hacer las pruebas de QA Manual (o exploratorias). Una vez que todos los cambios se encuentran en el trunk (rama develop), al finalizar las debidas tareas, se podrá subir todo a la rama main para hacer el release correspondiente.
## Políticas de pull request
Todo pull request deberá incluir:
- Nombre
- Descripción
- Issue que resuelve
- Evidencia visual (tests pasando o vídeo de la funcionalidad)
- Riesgos/Consideraciones (si aplica. Por ejemplo, una nueva migración)

# Configuración del repositorio en GitHub
## Archivos y directorios
El repositorio utilizará una estructura compuesta por: src/, docs/ y .github/. Este orden garantiza compatibilidad con las herramientas de CI/CD, y asegura que código, pruebas y documentación mantengan un historial de cambios, alineándose directamente con los principios de entrega continua que priorizan la predictibilidad y la trazabilidad.

En detalle, el repositorio utilizará una estructura donde src/ organizará el código por capas técnicas (domain, business logic, database, api), luego docs/ contendrá la documentación y .github/ alojará los pipelines de automatización, manteniendo los archivos de configuración en la raíz. Este orden garantiza compatibilidad con las herramientas de CI/CD, y asegura que código, pruebas y documentación mantengan un historial de cambios, alineándose directamente con los principios de entrega continua que priorizan la predictibilidad y la trazabilidad.
Por otro lado, no ubicaremos todos los tests en una única carpeta aislada ya que rompe con el principio de cohesión y dificulta la ejecución selectiva de pruebas de CI. Lo que haremois será mantener las pruebas junto a los módulos que validan o estructurar las pruebas en un directorio que replique la jerarquía del código fuente, lo que permite a las herramientas de CI ejecutar suites específicas, medir cobertura por módulo y fallar rápido sin escanear directorios innecesarios.


## Estrategia de branching
Para las ramas utilizaremos una adaptación de GitFlow con Continuous integration. Las ramas permanentes son  “main” que se tendrá el estado estable y desplegable del proyecto y “develop” que vamos a usar como rama de integración y es de esta que saldrán las otras ramas. Las ramas que se crearán desde “develop” son efímeras con prefijos semánticos:  “feature/” para nuevas funcionalidades, “fix/” para correcciones puntuales e inmediatos, “release/” para las entregas y “chore/” para mantenimiento no funcional. A su vez se tendrán ramas de tipo “docs/” en dónde se agregarán archivos de documentación varios. De este último tipo de ramas habrá una por entrega (en caso de ser necesario) para colocar evidencia de las actividades realizadas. Consideramos que la creación de más de una rama para este tipo de actividad aporta poco, ya que solo somos tres personas en el equipo, donde la mayoría de decisiones son tomadas en conjunto y si no fuera el caso, mantener la comunicación sobre ellas es sencillo. <br>
De esta forma logramos aislar la validación continua sin comprometer la rama principal,  alineándose con prácticas de integración temprana y retroalimentación rápida. Todas las ramas temporales tendrán una vida útil limitada a pocos días, minimizando la divergencia del historial, reduciendo el trabajo en progreso y facilitando la detección temprana de conflictos. Cada solicitud de fusión deberá superar pipelines de GitHub Actions que ejecuten suites de pruebas automatizadas antes de permitir la integración sobre “develop”, garantizando así que el historial de la rama principal permanezca siempre en estado desplegable. <br>
A su vez, se decidió que cuando sea necesario colocar más de una palabra como nombre de una rama se utilizará el caracter “_” como forma de separación.
## Estándares de nomenclatura
Para los commits utilizaremos Conventional Commits para garantizar flexibilidad y mantener la coherencia del proyecto, esto quiere decir que seguiremos el formato: \<tipo>(scope)\!: \<description\>.
- **tipo:** feat, docs, fix, test entre otras presentes en la [documentación oficial](https://www.conventionalcommits.org/en/v1.0.0/).
- **scope:** es opcional, palabra rápida para identificar que afecta el cambio. Ejemplos: api, db, ui, auth.
- **descripción:** breve frase sobre que se cambió.
- **!:** es solo necesario si se considera un “BREAKING CHANGE” que son cambios que introducen incompatibilidades con versiones anteriores.
# Análisis de deuda técnica y gestión de calidad
## Definición del modelo de calidad
Basándonos en el modelo de calidad [ISO 25010](https://iso25000.com/index.php/normas-iso-25000/iso-25010) definimos que nos centraremos especialmente en tres de los atributos: mantenibilidad, adecuación funcional y capacidad de interacción.
### Mantenibilidad
Este atributo es importante ya que en el contexto que nos encontramos la aplicación prevé tener cambios en un futuro, extendiendo su comportamiento. Por lo que necesitamos un alto grado de flexibilidad y claridad en el código. Así, permitiéndonos modificarlo con el mínimo impacto posible y que terceros puedan entenderlo de forma sencilla. <br>
Centrarnos en la mantenibilidad del sistema reducirá la deuda técnica y permitirá que las prácticas de CI/CD sean efectivas, ya que podremos asegurarnos que ningún cambio en nuestro repositorio común afecte la calidad del sistema. <br>
Para cumplir con este requisito las tácticas de TDD, Clean Code y patrones de diseño serán fundamentales.
### Adecuación funcional
El sistema que poseemos tiene reglas de negocio complejas, las cuales deben cumplirse para que los diferentes usuarios puedan realizar tareas críticas como el alta de medicamentos, el control de stock y la gestión de compras. Es de suma importancia que estas tareas, como algunas otras, se realicen de manera correcta y sin complicaciones, ya que es el enfoque principal de la aplicación. Si las compras no se ejecutan de manera adecuada, ya sea porque el medicamento no es el correcto o falta de medicamentos agregados al carrito, por ejemplo, perderíamos clientes. Lo que haría que el sistema no tuviera un propósito. <br>
Además, al tener varios tipos de usuarios distintos, con diferentes responsabilidades debemos asegurarnos que cada uno pueda realizar las operaciones que les corresponden sin errores, esto es algo que nos permite la adecuación funcional. <br>
Por ello es vital que el producto sea funcionalmente completo, cubriendo los casos de uso (completeness) y provea resultados exactos (correctness), para evitar errores.
### Capacidad de interacción
Como se trata de una página web sobre farmacias donde existen usuarios anónimos habrá una gran variedad de individuos de diferentes contextos accediendo (diferencias en: edad, habilidades, nivel socioeconómico, etc.), por lo que no conoceremos su identidad ni características principales. Este hecho es el que nos lleva a que el sistema debe ser lo más intuitivo posible, para que cualquier persona pueda realizar operaciones en la web con facilidad. Entonces, debemos prestarle atención a la inclusividad y la protección de errores del usuario. Esta última característica es relevante ya que un error al filtrar medicamentos con stock, por ejemplo, podrían frustrar al usuario. <br>
Todo esto se hará a través del seguimiento de las Heurísticas de Nielsen y el uso de programas como Wave (extensión del navegador utilizada para evaluar la accesibilidad de páginas web).
## Análisis de deuda técnica
### Mantenibilidad
#### Modularidad
Se presentan algunos problemas de modularidad; los managers (encontrados en Backend/PharmaGo.BusinessLogic) rompen el principio de responsabilidad única (SRP). Muchos de ellos mezclan autenticación, validación, lógica de negocio, acceso a datos (persistencia) y manejo de excepciones en un solo método. Por ejemplo: <br>
- En DrugManager.cs, el método Create(Drug drug, string token) (líneas 68-110) realiza validación de token, obtiene sesión/usuario/farmacia, verifica existencia de entidades relacionadas, asigna ID’s y persiste.
- En PurchasesManager.cs, CreatePurchase(Purchase purchase) (líneas 40-84) valida email, detalles de compra, calcula totales y genera códigos de tracking, todo en un solo lugar. <br>

Para mejorar en este aspecto debemos separar las responsabilidades. La autenticación y los validadores deberían realizarse en otros componentes. <br>
En el frontend podemos encontrar que todos los componentes, páginas y servicios están registrados en un único módulo (app.module.ts). Se deberían crear módulos para diferentes características o funcionalidades. Si se sigue así, la aplicación crecerá en un único lugar, dificultando la extensión o el reuso. <br>
**Impacto:** dificulta la extensión (ejemplo: agregar nuevas validaciones requiere tocar métodos largos) y reduce la claridad. Por lo que se impide que el sistema sea fácilmente divisible en partes independientes, aumentando el riesgo de errores en cambios futuros.
#### Reusabilidad
Se identifican problemas de reusabilidad asociados, principalmente, a la duplicación significativa en la lógica de autenticación. <br>
En el backend, la adquisición del token del usuario y el contexto nunca fue destinado a un método o validador en concreto, por lo que, cada vez que era necesario, se colocó el código en “crudo”. Entonces, el mismo código se encuentra repetido en:
- DrugManager.cs: líneas 76, 149 y 168.
- PurcharsesManager.cs: líneas 175 y 207.
- LoginManager.cs: líneas 46 (únicamente una parte) y 54. <br>

Esta duplicación no solo incrementa el esfuerzo de mantenimiento, sino que también introduce riesgo de inconsistencias entre implementaciones. Por lo que se debería centralizar la autenticación en un servicio y refactorizar los managers para que lo utilicen. <br>
En el frontend también nos encontramos que distintos servicios repiten la misma lógica de autorización (ejemplos: drug.service.ts, export.service.ts, invitation.service.ts), donde todos definen el método getHttpHeaders. Entonces, deberíamos implementar un interceptor para centralizar la inclusión de headers de autorización. <br>
A su vez, en varios servicios se repite la definición del método handleError<T>. <br>
Por otro lado, en el backend, existen dos formas diferentes de representar los estados ‘PENDING’, ‘REJECTED’ y ‘APPROVED’. En PurchasesManager.cs se encuentran como constantes y existe un enum que los contiene (StockRequestStatus). <br>
**Impacto:** se viola el principio DRY (Don’t Repeat Yourself), lo que hace que cambios en autenticación (ejemplo: agregar roles) requieran modificaciones en varios archivos. Reduce la reusabilidad, ya que no hay un servicio común de autenticación o helper. Esto afecta la capacidad de reutilizar componentes en futuros desarrollos.
#### Analizabilidad
Se identifican varios factores que reducen la facilidad para comprender el sistema, principalmente relacionados con complejidad e inconsistencias de diseño. <br>
En primer lugar, existen métodos excesivamente largos, lo que incrementa la complejidad y dificulta el seguimiento del flujo de ejecución. Por ejemplo: <br>
- UpdateInvitation en InvitationManager.cs (líneas 119-173).
- Create en DrugManager.cs (líneas 68-110). <br>

Deberíamos dividir estos métodos en funciones más pequeñas y cohesivas (cada una con una única responsabilidad). <br>
En segundo lugar, hay inconsistencias en el modelado del dominio. Los estados 'PENDING', 'REJECTED' y 'APPROVED' se encuentran duplicados (como constantes y como enum StockRequestStatus)  también afectan a la analizabilidad, ya que un desarrollador no sabrá con claridad cuál de las dos implementaciones debe utilizar en una nueva funcionalidad, lo que reduce la claridad del código. Por lo que debemos refactorizar el código para utilizar únicamente el enum existente. <br>
Además, la entidad Drug.cs define un validador, mientras que todas las demás entidades del dominio no poseen ninguno. Esto crea confusión, porque no se conocen claramente las responsabilidades. Para resolver este problema podríamos realizar validadores externos o colocarlos dentro de las entidades del dominio. <br>
Por último, desde el lado del frontend se observa el uso del tipo any (como en CommonService.ts). Esto reduce la capacidad de comprender el comportamiento del código únicamente a partir de su lectura, contradiciendo uno de los principales beneficios de TypeScript. Debemos reemplazar any por tipos explícitos. <br>
**Impacto:** esta combinación de factores harán que terceros (o incluso el equipo en un futuro) tarden más en entender el flujo: Además, la baja analizabilidad complica la identificación de defectos o mejoras, aumentando la deuda técnica.
#### Modificabilidad
En el backend, el alto acoplamiento entre managers y repositorios hace que los cambios sean propensos a efectos secundarios. Por ejemplo: <br>
- Managers inyectan múltiples repositorios: en el caso de managers como DrugManager y PurchasesManager, cada uno inyecta seis repositorios distintos. Por lo que, si se da un cambio en cualquiera de los repositorios de los que dependen, se verían afectados. Entonces, debemos agrupar lógica relacionada y delegar responsabilidades.
- Lógica repetida de obtener usuario: la lógica de obtener usuario y sesión desde un token se repite en varios managers (ejemplo: DrugManager y PurchasesManager). Esto fuerza a inyectar _sessionRepository y _userRepository en cada uno, por lo que, si se da un cambio en usuario, todos los managers que dependan de ellos se verían afectados. Entonces, encapsular la lógica de obtención de usuario y sesión sería algo positivo. <br>

**Impacto:** el alto acoplamiento actual provoca que cambios aparentemente locales (por ejemplo, modificaciones en reglas de validación o en la estructura de usuario) se propaguen a múltiples componentes, violando el principio de mínimo impacto.
#### Testeabilidad
Se identifican problemas de testeabilidad asociados, principalmente, a un alto acoplamiento, baja cohesión en métodos y cobertura de pruebas no uniforme. <br>
Existen varios métodos largos y acoplados que son difíciles de testear unitariamente. Un ejemplo de esto se da en CreatePurchase en PurchasesManager.cs depende de múltiples repositorios, lo que requiere de varios mocks. Así aumenta la complejidad de las pruebas y las vuelve frágiles ante cambios, ya que cualquier modificación en las dependencias puede romper varios tests. Por lo que se debe reducir el acoplamiento dividiendo métodos en unidades más pequeñas. <br>
Por otro lado, la cobertura de las pruebas unitarias llega al 91%, por lo que existen funciones que no poseen tests unitarios en diferentes paquetes. Si verificamos las pruebas unitarias, vemos que el paquete más afectado es WebApi, con 135 líneas sin coverage. A su vez, nos percatamos de que algunos repositorios (PresentationRepository, por ejemplo) no presentan sus respectivos tests. Entonces, a pesar del alto porcentaje global, existen partes críticas del sistema que no son validadas. Debemos crear test para dichas partes del negocio olvidadas. <br>
Impacto: la baja testabilidad impide verificar cambios de manera aislada, lo que contradice el énfasis en TDD; los componentes no testeables aumentan el riesgo de regresiones en CI/CD. <br>

- Incumplimiento de requisitos funcionales.
- Dificultad para encontrar rápidamente productos disponibles.
- Pérdida de valor para el usuario final.
**Causa:** implementación incompleta del listado de medicamentos; falta de desarrollo del filtro.
#### Deuda por falta de interfaz de usuario (UI/UX incompleta)
**Descripción:** acciones necesarias para el flujo normal de la aplicación no están expuestas en la interfaz. El dueño no tiene medios gráficos para gestionar stock requests, y los usuarios anónimos no pueden navegar a la pantalla de login mediante un botón.
**Impacto:**
- El sistema es difícil de usar (requiere conocimiento de rutas internas).
- Funcionalidad existente en backend queda inaccesible para el usuario final.
- Incrementa la frustración y abandono.
**Causa:** desarrollo centrado solo en la lógica de negocio, descuidando la capa de presentación y la experiencia de usuario.
#### Consecuencias a largo plazo
**Incremento de bugs en producción:** las validaciones faltantes generarán errores y datos corruptos.
**Mala reputación:** los usuarios percibirán el sistema como poco confiable e incómodo.
**Costo de cambio elevado**: cuanto más tiempo pase sin corregir estas omisiones, más costará integrar las soluciones debido a posibles acoplamientos.


#### Recomendaciones
- Implementar validaciones en el backend antes de concretar una compra y antes de crear invitaciones (verificar stock y unicidad de nombre de usuario).
- Agregar el filtro por disponibilidad de stock en la consulta de medicamentos (tanto en el servicio como en el frontend).
- Completar la interfaz del dueño añadiendo botones de aprobar/rechazar en la pantalla de stock requests.
- Incorporar un botón de login visible en la página principal (para usuarios anónimos).
- Establecer una política de validaciones obligatorias antes de dar por terminada una historia de usuario.
#### Conclusión
La deuda técnica identificada es funcional y de validación, con impacto directo en la experiencia de usuario y la integridad de los datos. Su corrección debería priorizarse en el próximo sprint, comenzando por las validaciones críticas (stock y duplicación de usuarios) para evitar problemas en producción, y continuando con las mejoras de UI que permitan una navegación y gestión completas.


### Capacidad de interacción
En lo que a capacidad de interacción del sistema, se evaluó que la aplicación sea intuitiva, inclusiva y proteja al usuario ante errores, con la esperanza de que la interfaz sea fácil de comprender, usable por varios perfiles y minimice errores de los usuarios. <br>
Como primera visualización, se denota que en el código la gran mayoría de los formularios están basados en placeholder sin tener etiquetas particulares, es decir, labels. Dicha ausencia reduce la usabilidad y la inclusividad porque el campo no es identificable cuando el usuario escribe en él y, además, los usuarios con dificultades cognitivas o visuales pierden contexto. Algunos formularios con el problema descrito son el de login y register. <br>
Otro problema de accesibilidad encontrado es que varias imágenes no contienen texto alternativo, es decir, el atributo alt. Es de por sí una violación a los principios de accesibilidad; en el caso de personas con lectores de pantalla, no recibirán descripción de ese contenido. Un ejemplo es dentro del componente home la not_found.png. <br>
Se encontró además que varios botones interactivos no cumplen con la accesibilidad esperada al no tener ni texto visible ni aria-labels como en botones del modal de estado de compra. Siendo que, además, en algunas ocasiones sí se encuentran aria-labels como en el campo de búsqueda, pero la utilización de labels o atributos ARIA no sucede de forma consistente. <br>
Por último, hay un mecanismo muy acotado de protección contra errores; por ejemplo, en el formulario de registro solo se encuentra un mensaje de “Password do not match”, sin comentar ni validar si hay requisitos o campos vacíos. Tampoco es evidente si el login falla en la autenticación o hubo algún otro tipo de error. <br>
Entonces, concluimos que, en términos de capacidad de interacción, existen varias debilidades que se pueden traducir a una mayor complejidad de uso de la interfaz para usuarios nuevos, con poca protección de errores y con mensajes poco claros. Tampoco es un desarrollo de interfaz muy inclusivo para personas con discapacidades visuales o cognitivas.
# Definición de issues y clasificación
# Retrospectiva y acciones de mejora

El equipo trabajó de forma organizada siguiendo los pasos y objetivos planteados. 
La mejora que creemos pertinente mejorar es tener más en cuenta el tiempo límite para optimizar las actividades y tiempos de los participantes.
