using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class CarrinhoDto
    {
        [DataMember]
        public virtual string IdCliente { get; set; }

        [DataMember]
        public virtual double ValorTotal { get; set; }
        [DataMember]
        public virtual double ValorTotalFrete { get; set; }
        [DataMember]
        public virtual double ValorTotalComFrete { get; set; }
        [DataMember]
        public virtual List<PedidoItemDto> Itens { get; set; }
    }

}
