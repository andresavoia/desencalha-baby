using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoStatusDto
    {
        [DataMember]
        public int CodPedidoStatus { get; set; }
        [DataMember]
        public string Titulo { get; set; }
    }


}
