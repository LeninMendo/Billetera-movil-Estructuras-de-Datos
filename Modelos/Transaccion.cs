using CajaArequipa.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Modelos
{/// Entidad principal que almacena la información detallada de una operación en la billetera móvil.
    public sealed class Transaccion
    {
        public string Codigo { get; }
    public TipoTransaccion Tipo { get; }
    public decimal Monto { get; }
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }
    public DateTime FechaRegistro { get; }
    public DateTime? FechaProcesamiento { get; set; }
    public EstadoTransaccion Estado { get; set; } = EstadoTransaccion.Pendiente;
    public string? CodigoReversa { get; set; }
    public string MensajeResultado { get; set; } = "Registrada y pendiente";

    public Transaccion(string codigo, TipoTransaccion tipo, decimal monto,
        string cuentaOrigen, string cuentaDestino)
    {
        Codigo = codigo.Trim();
        Tipo = tipo;
        Monto = monto;
        CuentaOrigen = cuentaOrigen.Trim();
        CuentaDestino = cuentaDestino.Trim();
        FechaRegistro = DateTime.Now;
    }

    public override string ToString() =>
        $"{Codigo} | {Tipo} | S/ {Monto:F2} | {Estado} | {FechaRegistro:dd/MM/yyyy HH:mm:ss}";
    }

}
