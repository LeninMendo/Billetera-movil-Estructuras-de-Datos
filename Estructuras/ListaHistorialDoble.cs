using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Estructuras
{
    /// Estructura de datos: Lista Doblemente Enlazada.
    /// Cada nodo posee punteros 'Anterior' y 'Siguiente', permitiendo recorrer el historial
    /// tanto en orden cronológico (antiguo a nuevo) como en orden inverso (nuevo a antiguo) de forma eficiente.
    public sealed class ListaHistorialDoble
    {
        private sealed class Nodo
        {
            public Transaccion Valor;
            public Nodo? Anterior;
            public Nodo? Siguiente;

            public Nodo(Transaccion valor)
            {
                Valor = valor;
            }
        }

        private Nodo? cabeza;
        private Nodo? cola;

        /// Inserta una nueva transacción al final de la lista doble O(1).
        public void Agregar(Transaccion transaccion)
        {
            var nuevo = new Nodo(transaccion);
            if (cabeza is null)
            {
                cabeza = nuevo;
                cola = nuevo;
            }
            else
            {
                nuevo.Anterior = cola;
                cola!.Siguiente = nuevo;
                cola = nuevo;
            }
        }

        /// Obtiene el historial de transacciones. Si 'inverso' es true, recorre desde la cola hacia la cabeza O(n).
        public List<Transaccion> ObtenerTodos(bool inverso = false)
        {
            var resultado = new List<Transaccion>();
            Nodo? actual = inverso ? cola : cabeza;

            while (actual is not null)
            {
                resultado.Add(actual.Valor);
                actual = inverso ? actual.Anterior : actual.Siguiente;
            }
            return resultado;
        }
    }
}
