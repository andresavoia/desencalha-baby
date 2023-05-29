using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class CriarPedidoResponse : BaseResponse
    {
        [DataMember]
        public virtual CriarPedidoDados Dados { get; set; }
    }

    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class CriarPedidoDados
    {
        [DataMember]
        public virtual int Id { get; set; }
    }
}
