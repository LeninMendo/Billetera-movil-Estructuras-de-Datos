using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Estructuras
{
    /// Estructura de datos: Cola FIFO (First-In, First-Out - Primero en entrar, primero en salir).
    /// Utilizada para procesar las transacciones operativas en el orden estricto en que fueron registradas.
    /// Inserción (Encolar) al final y extracción (Desencolar) al frente en O(1).
    public sealed class ColaTransacciones
    {
        private sealed class Nodo
        {
            public Transaccion Valor;
            public Nodo? Siguiente;

            public Nodo(Transaccion valor)
            {
                Valor = valor;
            }
        }

        private Nodo? frente;
        private Nodo? final;
        public int Cantidad { get; private set; }
        public bool EstaVacia => frente is null;

        /// Agrega una transacción al final de la cola (Enqueue) O(1).
        public void Encolar(Transaccion transaccion)
        {
            var nuevo = new Nodo(transaccion);
            if (final is null)
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final.Siguiente = nuevo;
                final = nuevo;
            }
            Cantidad++;
        }

        /// Extrae la transacción que está al frente de la cola (Dequeue) O(1).
        public Transaccion? Desencolar()
        {
            if (frente is null) return null;

            var valor = frente.Valor;
            frente = frente.Siguiente;

            if (frente is null)
            {
                final = null;
            }
            Cantidad--;
            return valor;
        }

        /// Retorna los elementos actuales pendientes en la cola sin eliminarlos.
        public List<Transaccion> ObtenerTodos()
        {
            var resultado = new List<Transaccion>();
            Nodo? actual = frente;
            while (actual is not null)
            {
                resultado.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return resultado;
        }
    }

}
