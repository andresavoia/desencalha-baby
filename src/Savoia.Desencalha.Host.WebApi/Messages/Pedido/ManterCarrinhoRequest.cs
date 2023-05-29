using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class ManterCarrinhoRequest
    {
        [DataMember]
        public virtual string UF { get; set; }
    }
}
