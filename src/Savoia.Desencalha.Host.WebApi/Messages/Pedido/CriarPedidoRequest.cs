using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class CriarPedidoRequest
    {
        [DataMember]
        public virtual string Observacao { get; set; }

        [DataMember(Name = "EnderecoEntrega")]
        public EnderecoDto EnderecoEntrega { get; set; }

    }
}
