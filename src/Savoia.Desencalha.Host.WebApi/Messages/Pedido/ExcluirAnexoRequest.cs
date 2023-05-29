using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class ExcluirAnexoRequest
    {
        [DataMember]
        public virtual int IdPedido { get; set; }
        [DataMember]
        public virtual PedidoAnexoDto Anexo { get; set; }
        [DataMember]
        public virtual string Observacao { get; set; }
        
    }
}

