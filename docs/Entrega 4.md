# Planificación y organización iniciales
#### Definición de roles
- **Desarrollo y test:** Shazira Giosa
- **Product owner:** Matías Ohanian
- **Coach Kanban:** Victoria Topolansky

#### Plan de la entrega
Para esta entrega, primero nos vamos a centrar en definir un nuevo tipo de testing relevante para nuestro modelo de calidad. Además, vamos a determinar las métricas de DevOps que se ajusten a nuestro proyecto. Por último, vamos a implementar lo solicitado en la letra respecto a la telemetría, que es un sistema de emitir logs, 2 métricas de negocio, 2 métricas a nivel de aplicación y 2 métricas a nivel de infraestructura.

# Métricas DevOps
## Métricas DORA
#### Frecuencia de despliegue (deployment frequency)
Dado que hay pocos despliegues en producción (main) ya que solo hacemos merge en la etapa final de cada entrega por el siguiente requisito: <br>
*“En el cierre de cada etapa se debe realizar merge a main en el repositorio GitHub y agregar un tag a la versión: ‘Entrega N’.”* <br>
No podemos tomar como tal esta métrica, ya que nos diría que hacemos menos de un despliegue cada dos semanas, tenemos que adaptarla. Por ello decidimos que calcularíamos la cantidad de veces en dos semanas que hacemos merge de manera exitosa en develop. Esto es así, ya que al ser merge exitosos de código en develop se traducirían en despliegues en producción. Dado que en la primera entrega no existía ningún tipo de código se contará desde la segunda.
- Segunda entrega del 21/04 al 04/05: 8 merge exitosos de código o del proceso CI/CD.  
- Tercera entrega del 05/05 al 25/05: 6 merge exitosos de código o del proceso CI/CD.
Entonces en total tenemos 5 semanas y 14 merges, entonces calculamos: <br>

`Deployment frequency = number of successful deployments (merges to develop) to production/number of weeks in the time period = 14/5 = 2,8` <br>
Por ende, tenemos casi 3 (2,8) despliegues exitosos por semana. <br>  
Según DevOps Handbook nos encontraríamos dentro de los High Performers ya que contamos entre un despliegue al día a uno por semana, pero tomando en cuenta que las entregas son cada dos o tres semanas y que se esperan solo cinco horas como mínimo de trabajo de cada uno de los tres miembros del equipo concluimos que la cantidad de despliegues exitosos que se realizan en promedio por semana podrían ser comparables a los de los Elite Performers (despliegan múltiples veces al día). Podemos concluir que el tamaño del lote del cambio del lote son, en general, pequeños (despliegues frecuentes) y se permite un movimiento rápido de las features.

#### Lead Time for Changes  
Para calcular esta métrica tomaremos el tiempo desde el primer commit en una rama hasta que la funcionalidad es integrada en develop (tomaremos develop en lugar de main por la justificación dada anteriormente) con éxito (si fue necesario un fix, luego de él). Calcularemos el Lead Time para las funcionalidades de la tercera entrega y calcularemos su promedio. <br>

`Lead Time for Changes = Deploy time (develop) - Commit time` <br>
`Average Lead Time for Changes = (Sum(Lead Time for Each Change))/Total Number of Changes` <br>

 Tiempos: <br>

1. Agregar test frontend:  
   * Primer commit: 17/05/2026 a las 09:31  
   * Integrado en develop correctamente: 18/05/2026 a las 18:26  
   * `Lead Time for Changes = 18/05/2026T18:26 - 17/05/2026T9:31 = 32:55 hs = 1975 min`  
2. Agregar Cypress y Cucumber al package.json:  
   * Primer commit: 18 de mayo de 2026 a las 19:51   
   * Integrado en develop correctamente: 18 de mayo de 2026 a las 20:22   
   * `Lead Time for Changes = 18/05/2026T20:22 - 18/05/2026T19:51 = 31 min`  
3. Feature crear reserva:  
   * Primer commit: 19 de mayo de 2026 a las 20:25  
   * Integrado en develop correctamente: 23 de mayo de 2026 a las 21:19   
   * `Lead Time for Changes = 23/05/2026T21:19 - 19/05/2026T20:25 = 1440 * 4 + 54 = 5814 min`  
4. Feature listar reservas:  
   * Primer commit: 24 de mayo de 2026 a las 01:54   
   * Integrado en develop correctamente: 25 de mayo de 2026 a las 13:15   
   * `Lead Time for Changes = 25/05/2026T13:15 - 24/05/2026T01:54 = 1440 + 660 = 2100 min` 
5. Feature retirar reserva:  
   * Primer commit: 24 de mayo de 2026 a las 13:20  
   * Integrado en develop correctamente: 25 de mayo de 2026 a las 18:44   
   * `Lead Time for Changes = 25/05/2026T18:44 - 24/05/2026T13:20 = 1440 + 324 = 1764 min` 
6. Feature confirmar reserva:  
   * Primer commit: 24 de mayo de 2026 a las 17:14   
   * Integrado en develop correctamente: 25 de mayo de 2026 a las 20:41  
   * `Lead Time for Changes = 25/05/2026T20:41 - 24/05/2026T17:14 = 1440 + 206 = 1646 min`

Calculamos el lead time promedio: <br>

`Average Lead Time for Changes = (1975 + 31 + 5814 + 2100 + 1764 + 1646)/6 = 13330/6 ≈ 2221,7 min` <br>

Entonces tenemos que el tiempo promedio desde que un cambio se inicia hasta que se integra correctamente en develop es aproximadamente 2221,7 minutos, lo que corresponde a un día y 13 horas aproximadamente. Esto nos indica que nos encontramos dentro de los High Performers según The DevOps Handbook, por ende tenemos margen de mejora. Pero es un buen promedio dado que no se trata de un trabajo dónde tenemos horas fijas de labor. Aunque sí es cierto que algunas funcionalidades contaron con una cantidad grande de commits y horas de trabajo que se podrían haber reducido y por ende reducir el tiempo promedio. Por ejemplo, el retirar una reserva se podría haber dividido en dos ramas distintas, una que permitiera retirar sin más y otra para agregar la funcionalidad de verificación con claves privadas y públicas.

#### MTTR
Dado que el sistema no cuenta con una base de incidentes históricos en producción dados por una carga de usuarios, decidimos que un cálculo de MTTR en este momento no nos daría datos reales y confiables sobre la capacidad del equipo y el sistema para detectar y resolver fallos. Sin embargo, la telemetría que se incluirá en esta entrega es un paso previo y necesario para poder medir el MTTR de manera efectiva en el futuro, dando diagnósticos rápidos y permitiendo la realización de Game Days en etapas posteriores.

#### Change Failure Rate  
En el caso de esta métrica si se decidió usar los despliegues en producción (merge a main), ya que contamos con una entrega que introdujo un error que no fue detectado antes de su despliegue y para la que fue necesario realizar un hotfix. <br>

`Change Failure Rate = (Number of failed deployments/Total number of production deployments) * 100` <br>

Tenemos tres despliegues en producción y uno de ellos fue fallido, por lo que: <br>

`Change Failure Rate = 13 * 100 ≈ 33,3` <br>

Según DevOps Handbook con esta change failure rate nos encontramos en la sección superior de los medium performers que abarca desde el 31% al 60%. Esto significa que existe trabajo no planificado y que tenemos que centrarnos en validar mejor los cambios que vamos a introducir en producción, ya sea con pruebas automáticas o realizando tests exploratorios con mayor profundidad.

## Métricas de flujo
#### Flow Efficiency  
`Flow Efficiency = (Process Time/Lead Time) * 100`

Tomaremos el Lead Time a partir del momento en el que comenzamos a trabajar sobre laentrega, ya que, como se trata de un obligatorio no podemos comenzar el mismo día que corresponde por calendario. <br>  
Lo calcularemos para la funcionalidad crear reserva de la entrega anterior sobre develop (por lo ya justificado en otras métricas): <br>

- Process time: 7 días  
- Lead Time: comenzamos el 09/05 y terminamos el 23/05. Entonces, `Lead Time = 14 días` 
- `Flow Efficiency = (7/14)*100 = 50%`

Por lo que pasamos un 50% del tiempo sin agregarle valor a esta funcionalidad, por ende tenemos que acortar los tiempos de entrega. Lo que se puede proponer es que por ejemplo, antes de empezar con el Example Mapping se podría haber hecho las actions para los tests y agregar la configuración necesaria para trabajar con Cypress y Cucumber en el frontend, eso nos hubiera ahorrado tres días del Lead Time, lo que hubiera dado un Flow Efficency del 63,6% aproximadamente, una mejora del 13,6%. Esto, claro, teniendo en cuenta que esta configuración se hiciera antes de comenzar la entrega, porque en el caso del obligatorio era en esta misma etapa dónde era requerido realizar dicha configuración.

#### Flow Distribution
Es la proporción de diferentes tipos de trabajo realizados en el sistema durante un período determinado.   
Vamos a tomar los trabajos realizados como Features (valor para el negocio) y deuda técnica (refactorizaciones, mejoras y mantenimientos).  
El período que se va a evaluar es desde el 21/04 al 25/05, donde nos basaremos en los pull request cerrados realizados en ese tiempo determinado. 
Para visualizar desde nuestro repositorio los pull request que consideramos, en el buscador de pull request se debe ingresar: [is:pr is:closed created:\>=2026-04-21 closed:\<=2026-05-25 base:develop](https://github.com/IngSoft-ISA2-2026-1/M7B-Giosa-Topolansky-Ohanian/pulls?q=is%3Apr+is%3Aclosed+created%3A%3E%3D2026-04-21+closed%3A%3C%3D2026-05-25+base%3Adevelop).   
Por lo tanto, por cada categoría se va a realizar la siguiente cuenta:  
`% Categoría = ((#PRs de la categoría) / (#Total de todos los PRs)) * 100`

Del período antes mencionado, solo se ignorarán 3 pull requests. Primero el #33 Chore/code analysis action front el cual es la incorporación del funcionamiento de las action con cypress. 
Luego, #39 Docs/entrega 2 y #49 Docs/entrega 3 los cuales representan actualizaciones de las documentaciones de las entregas y no son parte del sistema a analizar.  
Por lo tanto, nos resulta la siguiente tabla de pull requests junto a su categoría:

|  | PR | Título | Categoría |
| :---- | ----- | ----- | ----- |
| 1 | \#48 | Feat de confirmar reserva desde un empleado | Feature |
| 2 | \#47 | fix: error de cypress en actions (tests flaky) | Deuda técnica |
| 3 | \#46 | Feat/retirar reserva | Feature |
| 4 | \#45 | Feat/listar reservas | Feature |
| 5 | \#44 | feat: crear reserva | Feature |
| 6 | \#43 | Chore: cypress cucumber package.json | Deuda técnica |
| 7 | \#42 | fix: código eliminado de code-analysis.yml | Deuda técnica |
| 8 | \#41 | Chore/test frontend | Deuda técnica |
| 9 | \#38 | Feat/medicines buying quantity limit | Feature |
| 10 | \#37 | fix: actualizo versión del sdk de la api | Deuda técnica |
| 11 | \#36 | feat: botón para retroceder desde el login | Feature |
| 12 | \#35 | feat: botón login | Feature |
| 13 | \#34 | fix: versión del sdk .net en dockerfile | Deuda técnica |
| 14 | \#32 | chore: archivos para actions build y test | Deuda técnica |
| 15 | \#31 | chore: Agrego eslint y prettier | Deuda técnica |
| 16 | \#29 | chore: código para Code Analysis | Deuda técnica |

De todas los 16 pull requests, 7 responden a features y 9 a deuda técnica, siendo entonces la distribución de flujo:  
`Features = 7/16 = 43.75%`
`Deuda Técnica = 9/16 = 56.25%`
En base a los resultados obtenidos, se puede observar como el equipo tuvo más de la mitad de pull requests en la deuda técnica, lo que significa que se tuvo que incurrir en la configuración de herramientas, pruebas y análisis estático del proyecto para poder ofrecer el valor representado en las features. 
Se ponderó la calidad interna del proyecto antes que las funcionalidades implementadas para agregar al valor del negocio. Es contextualizable dentro de las semanas utilizadas para el seteo del pipeline y la cantidad de actions agregadas al proyecto en comparación al mucho tiempo dedicado en la técnica de Example Mapping, lo cual nos acotó la posibilidad de ofrecer más valor al negocio.

# Alpha y Beta testing aplicado a nuestro contexto
Dado que alpha testing requiere de encontrar errores, bugs y mejoras antes de desplegar la solución a producción, en nuestro contexto vamos a tomar que todos los errores que encontremos hasta la entrega anterior son elegibles para solucionar y realizar posteriormente Beta testing siendo que no tenemos una etapa real de producción con cliente reales. <br>
Por otro lado, el Beta testing consistirá en presentarle la solución con las mejoras elegidas aplicadas. Con esto buscamos obtener comentarios de mejora y reacciones para que en caso que sea necesario aplicarlos antes del final de la próxima entrega.   

## Errores encontrados en Alpha Testing de accesibilidad

El primer error detectado es la mala accesibilidad presentada por la página home, la cual es la primera vista de todos los clientes. El error mayoritario es la ausencia de descripciones alternativas (Alts) para las imágenes de los medicamentos. Por otro lado, se suma como error general que hay botones los cuales no refieren a ningún link. Los botones con errores son el de ver el carrito y para trackear nuestro pedido. Además hay problemas de contraste en el logo de la página y el elemento que cuenta los objetos dentro del carrito de la compra. 

![Errores en home](/docs/img/errores-en-home.png)

Al entrar en un medicamento se mantienen los mismos errores de la cabecera y se suma el error de texto alternativo de la imagen y otro error de contraste en el texto del botón agregar elemento a una reserva. 

![Errores en medicamento](/docs/img/errores-en-medicamento.png)

Para ver los carritos de compra y reserva se detectan 2 errores idénticos sobre cabecera de tabla vacía en los espacios para eliminar los elementos.

![Errores en cart](/docs/img/errores-en-cart.png)

También al entrar en el apartado de los empleados, los botones que te redirigen a diferentes apartados de las operaciones de los empleados, se detecta el error de link vacío. Se agrega además 2 errores más de contraste introducidos por los colores que estaban elegidos para los títulos.

![Errores en dashboard empleado](/docs/img/errores-en-dashboard-empleado.png)

Al acceder al apartado de crear medicamentos, se puede observar que todos los campos para ingresar carecen de etiquetas. 

![Errores en create drug](/docs/img/errores-en-create-drug.png)

El apartado de visualizar el estado de una compra presenta errores de headers de tabla y de contenido vacío además de botones vacíos. También hay errores de contraste para los colores en los estados.   

![Errores en purchases empleado](/docs/img/errores-en-purchases-empleado.png)

Luego de hacer toda esta investigación, se corrigieron los errores para lograr una página sin fallas graves de accesibilidad. Logramos que todas las páginas tengan un WAVE AIM Score promedio de 9/10, donde en algunos casos no es 10 porque los warnings o alerts no se solucionaron por priorizar solo los problemas con mayor impacto. Nuestra búsqueda principal de aplicar este tipo de testing es mejorar la capacidad de interacción del usuario, mejorando la accesibilidad para cualquier tipo de persona que acceda al sistema.

## Errores encontrados en Alpha Testing de flujos de usuarios

Para encontrar otros errores que se podría encontrar un usuario, probamos todos los flujos que este puede seguir actualmente en la aplicación. Al hacer esto, notamos que hay varios aspectos que podrían generar insatisfacción en los usuarios por ejemplo por falta de información que dificulta el uso o validaciones mal implementadas que generan respuestas inesperadas por parte de nuestro sistema. De esta forma logramos detectar varios fallos pero nos enfocamos en solucionar cinco de estos que consideramos afectan la experiencia de usuario. Todos los errores que encontramos se pueden ver en la siguiente tabla: <br>

| Tipo usuario | Descripción | Tipo error | Severidad | Solucionado |
| :---- | :---- | :---- | :---- | :---- |
| Cliente | Ingresar cantidad negativa en compra falla con error “Error The quantity must be not more than 20” | Feedback | Medio | No |
|  | Al aplicar realizar una búsqueda por nombre, para mostrar todo de nuevo se debe buscar vacío. botón de clear | Complica interacción | Medio | Si |
|  | Ingresar cantidad 0 en comprar, falla con mensaje de error “Error The quantity must be not more than 20” | Feedback | Medio | No |
|  | Tracking muestra búsquedas previas de compras que ya no están en la base de datos. Incluso si se carga la app sin BD muestra cosas, se guarda en el local storage pq si borras las cookies no se muestra. Si se buscan muestra “Error Get Purchase By Tracking Code code=YWAHTVB3GQ1EZGWN failed: Object reference not set to an instance of an object.” | No debería poder ver el usuario | Medio | No |
|  | Tracking, buscar codigo vacío el mensaje de error dice “Error Get Purchase By Tracking Code code= failed: Tracking Code is can't be empty ” y agrega una fila vacía | Feedback | Bajo/Medio | No |
|  | Tracking, cuando se busca un código incorrecto, en la tabla de recientes se muestra que este tiene un QR | Feedback | Medio/Alto | Si |
|  | Pick up en el id al poner número excesivamente grande da error que dice ”Error Get Booking failed: undefined ” | Feedback | Medio | No |
|  | Hacer booking te deja poner cantidad excesivamente grande | Feedback | Bajo | No |
|  | Hacer booking con todo el stock disponible de un medicamento falla. Deja crear el booking (que ahí suele fallar si no hay stock) pero al poner el mail y darle finish da error y muestra “Error Add Booking failed: The selected Quantity is more than available” | Lógica del sistema | Alto | Si |
| Administrador | Cuando se entra a Invitation list no hay ningún título que indique en qué sección se entró, solo se saca por contexto | Feedback | Medio | Si |
| Empleado | En purchase, cuando se rechaza una compra y se vuelve a ingresar a esta sección ya no se muestra la rechazada solo las pendientes o aprobadas | Feedback | Bajo | No |
|  | Cuando se entra a Purchase no hay ningún título que indique en qué sección se entró, solo se saca por contexto | Feedback | Medio | Si |


Para los clientes notamos que después de hacer una búsqueda de medicamentos, para mostrar toda la lista de nuevo se debe buscar vacío. Para mejorar la experiencia se agregó botón de clear search. Por otro lado en tracking, cuando se busca un código incorrecto o vacío, en la tabla de recientes se muestra que este tiene un QR, entonces lo cambiamos para que no se muestren. Al hacer una reserva con la cantidad de un medicamento igual a la cantidad total de stock que tiene no permite finalizarla. Deja crear el booking (que ahí suele fallar si no hay stock) pero al poner el mail y darle finish da error y muestra “Error Add Booking failed: The selected Quantity is more than available”. Con los cambios ahora si se permite hacer la reserva. <br>
Para los administradores cuando se entra a Invitation list no hay ningún título que indique en qué sección se entró, solo se saca por contexto. Simplemente se agregó un título descriptivo. <br>
Para los empleados el problema que encontramos era similar al de los administradores, cuando se entra a Purchase no hay ningún título que indique en qué sección se entró, solo se saca por contexto. Se soluciono agregando un título.

## Beta testing
Para esta parte del testing hicimos dos muestras de producto con usuarios diferentes y obtuvimos una gran cantidad de feedback. <br>
Primero, Alejandro de 43 años (daltonico) nos dio las siguientes anotaciones:
- La mayoria de los colores se distinguen bien pero, es confuso que los botones de agregar otro ítem al carrito y terminar la compra sean ambos azules
- El color naranja del botón de agregar medicamento a la reserva da la idea de precaucion mas que de agregar
- Estaria bueno tener un botón para agregar directo a la compra o reserva sin ver los detalles del producto 
- El eliminar de el carrito de compra no debería ser celeste
- Faltan botones de ir hacia atrás para mejorar la experiencia de usuario
- El botón de logout es confuso, queda como flotando en la ventana y se puede confundir con un botón de ir para atrás
- En los dashboards de los diferentes roles que se pueda tocar en todo el rectángulo para acceder a la funcionalidad, no solo al tocar en la flecha de “go”

Por otro lado, Victoria de 23 años notó que: 
- No está bueno poder ver los trackings de todos los usuarios
- Le gustaría que la búsqueda sea parcial, es decir no tener que ingresar el nombre exacto para ver encontrar lo que quieren 
- En los diferentes dashboard de funciones le molesta no poder hacer click en cualquier lugar del rectángulo para acceder.
- Le gustaría que las alertas del sistema se descarten solas después de un tiempo cuando se cambia de pestaña
- Estaria bueno tener la opción de poder ver lista de todas tus compras ingresando tu mail, por si se pierde el código
- Faltan botones de ir para atrás
- No se entiende como usar la seccion de crear stock request (como empleado), sería más claro si se pone afuera del rectángulo el botón de crear request
- Que el empleado pueda ver una lista con el stock de todos los medicamentos, no solo de los que creó un request para aumentar el stock
- Como se indica que hace cada función en los dashboards de los diferentes roles no es intuitivo y puede generar confusiones
- Algunos mensajes de error no indican claramente cuál es el problema o están redactados de forma confusa

Por último, junto a Andrea de 21 años conseguimos las siguientes situaciones:
- El botón de pagar es confuso al principio en la pestaña del carrito dado que también están los botones de reservas, por lo que encontrar el botón no fue directo.
- También notó que al mostrar nombres con tildes en la página, se mostraban como caracteres no reconocidos complicando el entendimiento de algunos textos.
- Al realizar una reserva, no era intuitivo a priori el objetivo de la private key mostrada por lo que fue necesario explicarle el fin.
-  Luego intentó levantar el medicamento pero tampoco era claro que antes la reserva además de realizarla se debía confirmar por un empleado. 
- Desde la óptica de un empleado, se dio cuenta que el apartado de export drug no funcionaba y daba error (el cual es existente desde el inicio).
- También quiso aprobar una compra realizada hace instantes y obtuvo un error de que el medicamento no se encontró en la farmacia por su código.
- Para crear una solicitud de stock, al poner un número excesivamente grande o un decimal el sistema falla con mensaje de error undefined. Mientras que logró permitió crear una solicitud con una cantidad negativa.

Concluimos, por tanto, según el feedbak recolectado que claramente nuestro sistema aún tiene varios puntos por mejorar con respecto a la usabilidad. Uno de los puntos que más afectaron la experiencia de usuario fue la navegabilidad por falta de botones para ir para atrás, sentimos que este es un punto que se podría mejorar para la próxima entrega.
También tendremos que realizar para asegurar las respuestas del sistema más casos de prueba en diferentes escenarios y estados. Otro punto es mejorar la intuición del sistema a primera vista al buscar esclarecer los motivos de cada parte presente. 
De todo esto podemos ver como claramente hay problemas que como desarrolladores al estar tan familiarizado con el funcionamiento interno del sistema los ignora, y solo se notan cuando se hacen pruebas con usuarios reales que nunca usaron la aplicación anteriormente. 


# Métricas de negocio, aplicación e infrastructura

Dentro de cada categoría de métrica, se determinó definir 2 particulares evaluadas según la utilidad y necesidad en base a nuestro sistema. Comenzando por las métricas de negocio, determinamos hacer en primer lugar la métrica `sum(completed_approved_purchases_total)` la cual es un counter que lleva el número acumulado total de compras donde el detalle de compra (lista de medicamento) tiene al menos un medicamento aprobado. 
Además, creemos que es muy valioso para las farmacias saber el valor promedio de las compras o el poder adquisitivo de sus compradores, por lo tanto decidimos crear la métrica average_ticket_buy de tipo Histograma el cual se compone de la siguiente forma: `sum(increase(approved_purchase_amount_currency_sum[1w])) / sum(increase(approved_purchase_amount_currency_count[1w]))`, donde significa el valor de la última semana total sobre la cantidad total de compras realizadas en el mismo período. 
Por el lado de las métricas de aplicación, decidimos tener en cuenta el promedio de tiempo que tardan las requests http, por lo tanto tomamos lo que ya estaba configurado y definimos la siguiente relación: `sum(http_server_request_duration_seconds_sum) / sum(http_server_request_duration_seconds_count)`. También consideramos importante tener en cuenta el porcentaje de peticiones http que resultaban en error para poder actuar instantáneamente, definido como: `(sum(http_server_request_duration_seconds_count{http_response_status_code=~"4..|5.."}) / sum(http_server_request_duration_seconds_count)) * 100`, donde el numerador consigue la cantidad de todas las peticiones que hayan resultado con código 400 o 500 y el denominador es la cantidad total de peticiones. 
Por último, determinamos para las métricas de infraestructura considerar el uso de cpu, llamada `rate(process_cpu_seconds_total[5m])` donde la utilizaremos para medir el tiempo total de segundos utilizado por el proceso en los últimos 5 minutos. La otra métrica de infraestructura definida es obtener el peso de la base de datos en la métrica database_size en base al uso del comando `sp_saceused`, siendo muy importante para el negocio saber cuanto pesa el total de la información almacenada dado que puede representar una posible inversión a corto plazo. 

## Dashboard en Grafana
De esta manera se ve nuestro dashboard en Grafana, el json del mismo se encuentra en el archivo "Dashboard Grafana".

![Dashboard en Grafana](/docs/img/dashboard-grafana.png)


## Problema detectado
Se ha identificado un comportamiento esperado pero limitante en Prometheus: la función `increase()` no puede calcular correctamente el incremento de una métrica cuando esta existe desde hace menos tiempo que la ventana de tiempo especificada en la consulta. Este fenómeno afecta a contadores. Los mismos son registrados por OPTLCollector la primera vez que su valor es distinto de cero y contienen en él el valor inicial de dicha llamada (como en nuestro caso obtiene valores cada 15 segundos, dependiendo sobre qué trabaje el contador puede ser que se pierda una o más actualizaciones, ya que son cargadas de forma simultánea) y provoca que consultas como `sum(increase(metric[2h]))` devuelvan `0` en el caso de que solo tengan dicho dato, en lugar del valor real acumulado (ej. `1`). Por otro lado, si el contador tiene dos entradas o más el resultado será: `valor_acumulado_actual - valor_acumulado_inicial`.
Esto sucede porque Prometheus almacena series temporales como una secuencia de pares (timestamp, valor). Un contador es una métrica monótonamente creciente. La función `increase(metric[time])` calcula el incremento total dentro de la ventana `time` mediante la siguiente lógica:

- Toma el valor más antiguo dentro de la ventana (`v_old`).
- Toma el valor más reciente dentro de la ventana (`v_new`).
- Devuelve `v_new - v_old`, ajustado por posibles reinicios del contador.

**Problema concreto:** Cuando una métrica aparece por primera vez en Prometheus, no existe ningún punto de datos anterior. Por lo tanto, dentro de cualquier ventana de tiempo que comience antes del nacimiento de la métrica, `v_old` no está definido. En estas condiciones, Prometheus no puede calcular un incremento fiable y, por diseño, devuelve 0.

### Ejemplo práctico
Si se realiza `http_server_request_duration_seconds_count` se puede observar como la suma del total es `7 + 2 + 1 + 5 + 1 = 16`.

![Server request inicial](/docs/img/server-request-inicial.png)

Ahora si ejecutamos `sum(increase(http_server_request_duration_seconds_count[2h]))`, vemos como el resultado se reduce a 7.

![Sum increase resultado](/docs/img/sum-increase-resultado.png)

Si vemos el primer valor en la parte gráfica, observamos que tenemos que el 7 comienza en 4, el 5 en 2 y luego los 3 restantes en 1. Así vemos como la diferencia entre la suma inicial (16) menos la suma de los valores iniciales (`4 + 2 + 3 = 9`) da efectivamente el resultado obtenido a través de increase (7).

![Gráfica valores server request](/docs/img/grafica-valores-server-request.png)

Este comportamiento no es un error, sino una decisión de diseño de Prometheus para evitar falsos incrementos. Sin embargo, representa una limitación operativa cuando las métricas tienen una vida corta o aparecen de forma dinámica.


### Posibles soluciones
Se evaluaron tres diferentes de soluciones. Todas presentan inconvenientes significativos en términos de complejidad, mantenimiento o cambios de infraestructura.

#### 1. Inicialización a cero en el cliente (C#)
**Descripción**  
Modificar el `Counter` para que devuelva explícitamente `0` en su inicialización, y solo después devuelva el valor real. Implementación mediante un flag `_firstScrape`.

**Ventajas**
- Solución limpia desde la perspectiva de Prometheus: la serie temporal comienza con `0`, luego `1`, etc.
- `increase()` funciona correctamente desde el primer momento.

**Inconvenientes / complejidades**
- Requiere mantener estado por cada contador (flag booleano).
- Si la aplicación se reinicia, se emite un nuevo `0` que puede interpretarse como un reinicio real del contador, que distorsionaría las tasas a largo plazo.
- La implementación de la flag para no tener problemas de falsos reinicios significaría el guardado del último valor en la base de datos, por ejemplo, y cada vez que se reinicie el sistema obtener dicho valor para comprobar si el contador ya había comenzado o no.

**Conclusión:** la complejidad de estado y los efectos colaterales no justifican su adopción.

#### 2. Soluciones en PromQL
**Descripción**  
Utilizar operadores como `or` o funciones no estándar para “inventar” un valor cero cuando la métrica no tiene historia.

**Algunos ejemplos evaluados**
- `sum(increase(metric[2h]) or metric)`
- `sum((increase(metric[2h]) > 0) or metric)`
- `sum(default_zero(metric[2h]))` (función inexistente en PromQL estándar)

**Ventajas**
- No requieren cambios en el código de la aplicación.
- Si funcionaran, serían transparentes para los equipos de desarrollo.

**Inconvenientes / complejidades**
- Las consultas simples con `or` fallan cuando el contador supera el valor `1` (porque `increase()` devuelve `0` y el `or` no se activa).
- `default_zero()` no existe; es una función de algunas bibliotecas, no de Prometheus nativo.

**Conclusión:** las soluciones en PromQL son frágiles, poco legibles y propensas a errores. Mantener dashboards con estas expresiones sería insostenible.

#### 3. Cambio de herramienta de monitoreo

**Descripción**  
Migrar a un sistema que maneje este caso de forma nativa, como VictoriaMetrics, que ofrece `increase_pure()`. Esta función asume que el primer valor de una métrica es un incremento desde `0`.

**Ventajas**
- Solución definitiva y soportada por la herramienta.
- Sin cambios en el código de la aplicación.

**Inconvenientes / complejidades**
- Migrar todo el stack de monitoreo (Prometheus, Alertmanager, Grafana) a VictoriaMetrics es complejo e implica no cumplir con la letra del obligatorio.

**Conclusión:** no se realizará para mantenernos sobre lo pedido.

### Justificación de la decisión
- El comportamiento es estándar y documentado por Prometheus, por ende no es un fallo.
- Las soluciones disponibles introducen una complejidad desproporcionada (estado en cliente, consultas PromQL frágiles, migración de infraestructura) en relación con el impacto real del problema.
- El caso de uso afectado (consultas con ventanas de tiempo mayores a la antigüedad de la métrica) es relativamente infrecuente y puede mitigarse mediante buenas prácticas (ej. usar `increase()` con ventanas adaptadas al ciclo de vida esperado de las métricas).
- Documentar esta limitación es más eficiente que implementar soluciones complejas que podrían introducir nuevos errores.
- Una mitigación práctica es utilizar ventanas cortas en lugar de ventanas largas cuando se sabe que la métrica es nueva, o basar dashboards en el valor absoluto (metric) en lugar de increase si solo se necesita el total acumulado.
