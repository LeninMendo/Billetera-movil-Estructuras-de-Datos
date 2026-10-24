using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Estructuras
{
    /// Estructura de datos: Pila LIFO (Último en entrar, primero en salir).
    /// Utilizada para gestionar solicitudes de reversión de transacciones, donde la última
    /// solicitud registrada es la primera en ser atendida.
    public sealed class PilaReversas
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

        private Nodo? cima;
        public int Cantidad { get; private set; }
        public bool EstaVacia => cima is null;

        /// Apila una solicitud de reversión en la cima de la pila (Push) O(1).
        public void Apilar(Transaccion transaccion)
        {
            var nuevo = new Nodo(transaccion) { Siguiente = cima };
            cima = nuevo;
            Cantidad++;
        }

        /// Desapila la solicitud de reversión más reciente (Pop) O(1).
        public Transaccion? Desapilar()
        {
            if (cima is null) return null;

            var valor = cima.Valor;
            cima = cima.Siguiente;
            Cantidad--;
            return valor;
        }

        /// Retorna todas las transacciones pendientes en la pila.
        public List<Transaccion> ObtenerTodos()
        {
            var resultado = new List<Transaccion>();
            Nodo? actual = cima;
            while (actual is not null)
            {
                resultado.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return resultado;
        }

        /// Verifica si un código ya se encuentra registrado en la pila de reversas.
        public bool ContieneCodigo(string codigo)
        {
            Nodo? actual = cima;
            while (actual is not null)
            {
                if (actual.Valor.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }
    }

}
