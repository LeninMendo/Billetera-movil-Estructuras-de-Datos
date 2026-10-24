using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Estructuras
{
    /// Estructura de datos: Lista Enlazada Simple.
    /// Cada nodo contiene un objeto Transacción y una referencia al siguiente nodo.
    /// Ventaja: Inserción eficiente al final O(1) manteniendo un puntero 'cola', y recorrido secuencial O(n).
    public sealed class ListaTransacciones
    {
        private sealed class Nodo
        {
            public Transaccion Valor;
            public Nodo? Siguiente;

            public Nodo(Transaccion valor)
            {
                Valor = valor;
                Siguiente = null;
            }
        }

        private Nodo? cabeza;
        private Nodo? cola;
        public int Cantidad { get; private set; }

        /// Agrega una transacción al final de la lista enlazada en tiempo O(1).
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
                cola!.Siguiente = nuevo;
                cola = nuevo;
            }
            Cantidad++;
        }

        /// Verifica si un código de transacción ya existe recorriendo la lista secuencialmente O(n).
        public bool ExisteCodigo(string codigo)
        {
            Nodo? actual = cabeza;
            while (actual is not null)
            {
                if (string.Equals(actual.Valor.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }

        /// Retorna una lista estándar con todas las transacciones para visualización en la interfaz.
        public List<Transaccion> ObtenerTodos()
        {
            var resultado = new List<Transaccion>();
            Nodo? actual = cabeza;
            while (actual is not null)
            {
                resultado.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return resultado;
        }
    }

}
