using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Domain.Models
{
    public class FreteEstadoModel
    {
        public string IdCliente { get; set; }
        public string IdFreteEstado { get; set; }
        public string UF { get; set; }
        public double Valor { get; set; }
        public double? ValorPedidoFreteGratis { get; set; }
        public int? DiasPrazoEntrega { get; set; }

    }
}
