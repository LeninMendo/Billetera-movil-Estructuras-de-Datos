using CajaArequipa.Estructuras;
using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Servicios
{
    public sealed class GestorTransacciones
    {

            public ListaTransacciones Transacciones { get; } = new();
            public ListaHistorialDoble Historial { get; } = new();
            public ColaTransacciones Pendientes { get; } = new();
            public PilaReversas SolicitudesReversa { get; } = new();
            public Dictionary<string, decimal> Saldos { get; } = new(StringComparer.OrdinalIgnoreCase);

            public GestorTransacciones()
            {
                // Inicialización de cuentas con saldos base simulados
                Saldos["CTA001"] = 1000m;
                Saldos["CTA002"] = 500m;
                Saldos["CTA003"] = 250m;
            }

            /// Registra una nueva transacción validando reglas y distribuyéndola en las estructuras de datos correspondientes.
            public Transaccion Registrar(string codigo, TipoTransaccion tipo, decimal monto,
                string origen, string destino)
            {
                if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("El código es obligatorio.");
                if (Transacciones.ExisteCodigo(codigo)) throw new ArgumentException("Ya existe una transacción con ese código.");
                if (monto < 0 || (tipo != TipoTransaccion.Consulta && monto <= 0))
                    throw new ArgumentException("El monto debe ser mayor que cero para abonos y transferencias.");
                if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
                    throw new ArgumentException("Indica las cuentas de origen y destino.");
                if (tipo == TipoTransaccion.Transferencia && origen.Equals(destino, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("La cuenta de origen y destino deben ser diferentes.");

                AsegurarCuenta(origen);
                AsegurarCuenta(destino);

                var t = new Transaccion(codigo, tipo, monto, origen, destino);

                // Distribución en las estructuras de datos
                Transacciones.Agregar(t);         // Lista enlazada simple
                Historial.Agregar(t);             // Lista doble enlazada
                Pendientes.Encolar(t);            // Cola FIFO

                return t;
            }

            /// Procesa la siguiente transacción en la cola FIFO aplicando las reglas de saldo correspondientes.
            public Transaccion? ProcesarSiguiente()
            {
                var t = Pendientes.Desencolar();
                if (t is null) return null;
                if (t.Estado != EstadoTransaccion.Pendiente) return t;

                try
                {
                    switch (t.Tipo)
                    {
                        case TipoTransaccion.Abono:
                            AsegurarCuenta(t.CuentaDestino);
                            Saldos[t.CuentaDestino] += t.Monto;
                            break;
                        case TipoTransaccion.Transferencia:
                            AsegurarCuenta(t.CuentaOrigen);
                            AsegurarCuenta(t.CuentaDestino);
                            if (Saldos[t.CuentaOrigen] < t.Monto)
                                throw new InvalidOperationException("Saldo insuficiente en la cuenta de origen.");
                            Saldos[t.CuentaOrigen] -= t.Monto;
                            Saldos[t.CuentaDestino] += t.Monto;
                            break;
                        case TipoTransaccion.Consulta:
                            AsegurarCuenta(t.CuentaDestino);
                            t.MensajeResultado = $"Consulta completada. Saldo de {t.CuentaDestino}: S/ {Saldos[t.CuentaDestino]:F2}";
                            break;
                    }
                    t.Estado = EstadoTransaccion.Procesada;
                    if (t.Tipo != TipoTransaccion.Consulta)
                    {
                        t.MensajeResultado = "Procesada correctamente";
                    }
                }
                catch (InvalidOperationException ex)
                {
                    t.Estado = EstadoTransaccion.Fallida;
                    t.MensajeResultado = ex.Message;
                }

                t.FechaProcesamiento = DateTime.Now;
                return t;
            }

            /// Busca una transacción recorriendo la lista enlazada simple: O(n).
            public Transaccion? Buscar(string codigo) =>
                Transacciones.ObtenerTodos().FirstOrDefault(t =>
                    string.Equals(t.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

            /// Solicita la reversión de una transacción procesada y la apila en la Pila LIFO.
            public void SolicitarReversa(string codigo)
            {
                var t = Buscar(codigo) ?? throw new ArgumentException("No se encontró el código.");
                if (t.Tipo == TipoTransaccion.Consulta)
                    throw new InvalidOperationException("Las consultas no modifican saldo y no se revierten.");
                if (t.Estado != EstadoTransaccion.Procesada)
                    throw new InvalidOperationException("Solo se puede revertir una transacción procesada.");
                if (!string.IsNullOrEmpty(t.CodigoReversa) || SolicitudesReversa.ContieneCodigo(codigo))
                    throw new InvalidOperationException("La transacción ya tiene una reversa aplicada o solicitada.");

                SolicitudesReversa.Apilar(t);
            }

            /// Procesa la solicitud de reversión más reciente utilizando la Pila LIFO.
            public Transaccion? ProcesarReversaMasReciente()
            {
                var t = SolicitudesReversa.Desapilar();
                if (t is null) return null;

                if (t.Estado != EstadoTransaccion.Procesada || !string.IsNullOrEmpty(t.CodigoReversa))
                {
                    t.MensajeResultado = "La reversa ya no es válida.";
                    return t;
                }

                if (t.Tipo == TipoTransaccion.Abono)
                {
                    if (Saldos[t.CuentaDestino] < t.Monto)
                    {
                        SolicitudesReversa.Apilar(t); // Re-apilar si no se pudo procesar por saldo
                        throw new InvalidOperationException("No hay saldo suficiente para revertir el abono.");
                    }
                    Saldos[t.CuentaDestino] -= t.Monto;
                }
                else
                {
                    if (Saldos[t.CuentaDestino] < t.Monto)
                    {
                        SolicitudesReversa.Apilar(t);
                        throw new InvalidOperationException("La cuenta destino ya no tiene saldo suficiente para revertir la transferencia.");
                    }
                    Saldos[t.CuentaDestino] -= t.Monto;
                    Saldos[t.CuentaOrigen] += t.Monto;
                }

                t.Estado = EstadoTransaccion.Revertida;
                t.CodigoReversa = "REV-" + t.Codigo;
                t.MensajeResultado = $"Revertida mediante {t.CodigoReversa}";
                return t;
            }

            private void AsegurarCuenta(string cuenta)
            {
                if (!Saldos.ContainsKey(cuenta)) Saldos[cuenta] = 0m;
            }

    }
}
