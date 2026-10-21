using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaArequipa.Servicios
{
    public sealed class GestorTransacciones
    {

        public Dictionary<string, decimal> Saldos { get; } = new(StringComparer.OrdinalIgnoreCase);
        public GestorTransacciones()
        {
            Saldos["CTA001"] = 1000m;
            Saldos["CTA002"] = 500m;
            Saldos["CTA003"] = 250m;
        }

    }
}
