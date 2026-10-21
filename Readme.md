<h1 align="center"> BilleteraMovil - Estructura de Datos </h1>
<h3 align="center"> Simulador de billetera móvil con estructuras de datos </h3>

🧰 Herramientas usadas

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=visualstudio,cs,dotnet,git,github,&theme=light" />
  </a>
</p>

Aplicación de escritorio en C# Windows Forms para simular operaciones de una billetera móvil y demostrar estructuras de datos.

## Funcionalidades
- Registrar abonos, transferencias y consultas.
- Cola FIFO para procesar operaciones en orden de llegada.
- Lista enlazada simple para el registro de transacciones.
- Lista doble para recorrer el historial en ambos sentidos.
- Árbol binario de búsqueda por código.
- Pila LIFO para solicitudes de reversión.
- Lista circular para simular reintentos limitados de sincronización.
- Grafo ponderado de agencias y búsqueda de ruta de menor costo con Dijkstra.
- Saldos simulados en memoria.

## Cuentas iniciales
- CTA001: S/ 1,000.00
- CTA002: S/ 500.00
- CTA003: S/ 250.00

Las cuentas no registradas se crean con saldo cero al registrar una operación.


## Límites y alcance
- Es una simulación académica; no mueve dinero real, no se conecta a redes bancarias.
- Los datos se guardan en memoria y se pierden al cerrar la aplicación.
- Los costos de las rutas son ficticios y no representan distancias ni conectividad real.

## Estructura
| Estructura | Operación  |
|---|---|
| Lista enlazada simple | Agregar al final con referencia a cola | 
| Lista doble | Agregar al final | 
| Cola FIFO | Encolar/desencolar | 
| Pila LIFO | Apilar/desapilar | 
| Árbol BST | Buscar por código | 
| Lista circular | Recorrer hasta límite |
| Grafo + Dijkstra | Ruta de menor costo |

## Estructura de carpetas
- `Modelos`: transacción y enumeraciones.
- `Estructuras`: estructuras implementadas manualmente.
- `Servicios`: reglas de negocio y coordinación.
- `Interfaz`: formulario principal.