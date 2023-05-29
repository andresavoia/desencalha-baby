using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Frete
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/frete/types/")]
    public class FreteEstadoDto
    {
        [DataMember]
        public string IdFreteEstado { get; set; }
        [DataMember]
        public string UF { get; set; }
        [DataMember]
        public double Valor { get; set; }
        [DataMember]
        public double? ValorPedidoFreteGratis { get; set; }

        [DataMember]
        public int? DiasPrazoEntrega { get; set; }
    }
}
