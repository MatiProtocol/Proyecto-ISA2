# Planificación y organización iniciales
#### Definición de roles
- **Desarrollo y test:** Matías Ohanian
- **Product owner:** Victoria Topolansky
- **Coach Kanban:** Shazira Giosa
#### Plan de la entrega
Acordamos que comenzaríamos definiendo primero el pipeline de la solución de forma grupal. Cuando se finalizara con dicha tarea y los pasos automáticos implementados en el repositorio de GitHub, se haría la elección de ambos bugs y su implementación.
# Pipeline
Con todo el grupo se acordó que de manera el pipeline general sería de la siguiente manera:

![Pipeline](/docs/img/Pipeline.png)
- **Desarrollo**: la etapa de desarrollo abarca toda la implementación del código hasta que se consigue implementar una funcionalidad.
- **Formatting**: etapa manual dónde se corren diferentes comandos para que el código cumpla con estándares de codificación.
- **Commit**: se crea un Pull Request a develop de la rama que queremos integrar.
- **Build**: es una parte de un GitHub Action que comprueba que la solución pueda ser construida automáticamente. Tanto el Frontend como el Backend se construyen en simultáneo.
- **Test**: otra parte de una GitHub Action que comprueba que todos los test de la solución sean coherentes y exitosos. Además, se realiza un análisis de cobertura de código con la condición de que la cobertura sea superior a 85% para que la action pase. De todas formas, la cobertura debería estar en torno al 90% para asegurar que la amplia mayoría del código que va a llegar a producción es funcional y correcto. Esta Action debe correr siempre luego de la encargada de construir la solución.
- **Code Analysis**: GitHub Action que verifica que el formato del código sea correcto. Decidimos que la GitHub Action de Code Analysis se haga de manera paralela así obteniendo una retroalimentación de manera más rápida.
- **QA Manual**: cuando las nuevas funcionalidades, o modificaciones a las ya existentes, son integradas a la rama develop (a través del pull request anterior) se deben hacer pruebas exploratorias. Ellas se harán de manera manual verificando que las funcionalidades sean accesibles para el usuario y que se comporten de la manera esperada. Así, aportamos valor a dónde la automatización no llega y devolviendo feedback de forma rápida.
- **Despliegue**: una vez el código fue validado tanto por las pruebas automáticas como por el QA Manual es momento de que sea desplegado en producción (se crea un pull request a main integrando los cambios allí).

Se pensó sobre implementar una action que luego de terminar con test y code analysis cree un contenedor en Docker de manera automática para que los miembros del equipo tengan que hacer solo 'docker-compose up -d' para probar el código en QA Manual usando probablemente Docker Hub. Pero luego de razonar el fin de utilizar dicha función, llegamos a la conclusión de que, por ejemplo, en dos ramas que salen de develop a la vez, cuando se realice el Pull Request de la primera se crearía una imagen con los cambios y cuando se realiza el segundo Pull Request se crearía otra nueva imagen sin los cambios de la anterior rama. En definitiva, estaríamos gastando recursos en las actions y a su vez es preferible que se realice la imagen con todos los cambios al momento de realizar el QA. Se definió así porque haremos pocos cambios en la aplicación y ellos serán bastante pequeños.


## Automáticas
Se decidió usar los runners propios de GitHub, ya que con el plan gratis de GitHub contamos con 2000 minutos (aproximadamente 33 horas) de uso por mes por cuenta (dato obtenido de: GitHub Docs). No se piensa utilizar esta opción para nada más durante el transcurso del proyecto por lo que decidimos que sería la mejor opción.
<br>
El pipeline automático se compone de dos workflows, build-test.yml y code-analysis.yml, que se ejecutan en paralelo entre sí ante cada Pull Request y ante pushes a main o develop. Cada workflow se activa únicamente si el push modifica archivos relevantes para cada uno como pueden ser por ejemplo .csproj o package.json para build-test.yml y .js, .jsx para el code-analysis.yml. Dentro de cada workflow, el job de backend y el de frontend corren también en paralelo. Específicamente los pasos se verán de la siguiente manera:

![Pipeline automatico](/docs/img/pipeline-automatizacion.png)

### Build y Test
El workflow build-test.yml delega en dos workflows reutilizables. Para el backend invoca reusable-build-test-backend.yml apuntando a ./src/Backend/PharmaGo.sln y para el frontend invoca reusable-build-frontend.yml sobre ./src/Frontend.
<br>
El backend se divide en dos jobs con dependencia explícita, primero, test no comienza hasta que build finaliza con éxito y segundo, el job build ejecuta dotnet restore seguido de dotnet build --configuration Release. El job test ejecuta los tests y guarda los resultados, el job falla si algún test no pasa o si la cobertura de los tests no supera el 85%. Una vez se tienen los resultados se analizan y se genera un reporte de cobertura para que se pueda ver claramente el estado de la solución.
<br>
El job de frontend configura Node.js 20.x con caché de npm vinculado al package-lock.json, ejecuta npm install y luego npm run build -- --configuration production. El job falla si la compilación de producción de Angular da error.

### Code Analysis
El workflow code-analysis.yml utiliza reusable-code-analysis-backend.yml y reusable-code-analysis-frontend.yml.
<br>
El job de backend ejecuta dotnet format sobre PharmaGo.sln con las flags --verify-no-changes, --verbosity diagnostic y --report, que genera el archivo format-report.json. La flag continue-on-error: true permite que se ejecute siempre el código que sigue a pesar de que algo falle, esto es útil ya que el siguiente código construye una tabla con columnas File, Line, Diagnostic y Description para que un desarrollador pueda identificar claramente los errores del código. Es solo si se detecta algún problema que el reporte se sube al pull request.
<br>
El job de frontend corre ESLint y Prettier de forma independiente, ambos también con continue-on-error: true para que siempre se terminen de ejecutar. ESLint se lanza con --max-warnings 0 y salida en JSON (eslint-report.json), y Prettier con --check, capturando la salida en prettier-output.txt. Después se parsean ambos archivos y se obtiene la información crítica, es con esta información que se construye una tabla unificada que muestra File, Line, Column,  Tool y Description. Como en el otro job, si se encuentran errores se publica la tabla.


## Manuales
### Formatting
Antes de abrir un Pull Request, el desarrollador debe ejecutar localmente las herramientas de formateo para que el código ya llegue correctamente formateado a la pipeline. <br>
Dentro del backend “dotnet format PharmaGo.sln”, y por otro lado, para el frontend se utiliza “npx eslint . --fix” y “npx prettier --write .” ESLint corrige problemas de linting y Prettier normaliza el estilo de formato. Estos comandos solucionan la mayoría de problemas pero, es probable que el desarrollador deba solucionar manualmente algunos de los errores encontrados por las herramientas. <br>
Se evaluó la incorporación de StyleCop.Analyzers como herramienta de análisis estático para el backend. Sin embargo, se decidió no utilizarla ya que su configuración inicial genera una gran cantidad de advertencias que requieren ser ajustadas manualmente mediante un archivo stylecop.json y un .ruleset personalizado. Este es un proceso que además puede generar inconsistencias entre el IDE y el CI cuando hay diferencias de versiones del SDK. Dado el alcance académico del proyecto, el costo de configuración no resultaba justificado. En su lugar, se optó por dotnet format con .editorconfig, que cubre las necesidades de consistencia de estilo con una configuración considerablemente más simple.

### QA Manual
Esta etapa consiste de Pruebas Exploratorias y UAT (Pruebas de Aceptación de Usuario). El QA se hace en la etapa final del ciclo para no desperdiciar horas humanas, de esta forma nos aseguramos que solo se valide manualmente cuando el cambio ya se testeo de varias formas. 
Primero las pruebas exploratorias buscan que los tester prueben caminos y comportamientos alternativos para validar el funcionamiento del sistema. De esta forma se logran detectar errores complejos que los tests automáticos no llegan a capturar.
Por otro lado, las pruebas UAT consistirán en validar con el product owner como parte de negocio que el sistema realiza lo que realmente se necesita. Entonces, cada funcionalidad que integremos al sistema debemos asegurarnos que cumple con los criterios de aceptación y además aporta el valor que espera el cliente. 


# Bugs elegidos y resueltos
Elegimos los issues de [acceso al login](https://github.com/IngSoft-ISA2-2026-1/M7B-Giosa-Topolansky-Ohanian/issues/1) y [manejo de cantidades de medicamento](https://github.com/IngSoft-ISA2-2026-1/M7B-Giosa-Topolansky-Ohanian/issues/25), ambos tienen prioridad alta en nuestro tablero de Kanban. A nuestro criterio estos son bugs críticos a resolver ya que van directamente en contra de los atributos de calidad que definimos como importantes para nuestro proyecto, específicamente la Adecuación funcional y Capacidad de interacción. Primero, el acceso al login afecta directamente la usabilidad de los usuarios Administradores, Dueños y Empleados ya que al no contar con un botón de login estos no pueden acceder a sus funciones críticas. Dejar a un usuario sin poder utilizar ciertas funciones se relaciona con Adecuación funcional y esperar que este cambie el URL de la aplicación para utilizar una función va directamente en contra de lo planteado en la Capacidad de  interacción. <br>
Por otro lado, relacionado al manejo de cantidad de medicamentos, también impacta directamente al usuario ya que podría generar que un usuario que pagó por un medicamento no pueda obtenerlo por falta de stock. Esta situación afecta negativamente al usuario que requiere un medicamento y a la reputación de la farmacia. Esto se identifica como un claro problema del atributo Adecuación funcional ya que va en contra de las reglas del negocio por falta de validaciones. <br> 
También, nos parece relevante destacar que solucionamos el bug [test Test_GetOneByExpression falla al asumir un Id](https://github.com/IngSoft-ISA2-2026-1/M7B-Giosa-Topolansky-Ohanian/issues/22). Fue necesario solucionarlo para seguir con la automatización del pipeline, ya que si un test falla el sistema no permite avanzar. Este bug afecta directamente la mantenibilidad del sistema, ya que una prueba que está mal definida afecta la testabilidad y analizabilidad del mismo. Decidimos que al ya tener un issue definido lo mejor era modificar el test para que pasara adecuándose al comportamiento esperado, en lugar de su eliminación.

# Retrospectiva
En un principio habíamos pautado hacer reuniones semanales de avance, pero en el transcurso del proyecto nos fuimos dando cuenta que para nuestro equipo esto se sentía forzado y no aportaba valor. Concluimos que esto nos sucede ya que somos un equipo pequeño, estamos en constante comunicación y ya hay establecido un nivel de confianza, por lo tanto cuando un miembro se asigna una tarea sabemos que cuando la termine va a avisar. Además, si surge alguna duda utilizamos una técnica similar a Andon cord preguntando inmediatamente por un grupo de WhatsApp. De esta forma, cada uno puede avanzar por su cuenta y todos sabemos el estado del proyecto, sin sorpresas de último momento.


