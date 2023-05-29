using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class ManterPedidoResponse : BaseResponse
    {
        [DataMember]
        public virtual int Id { get; set; }

        [DataMember]
        public virtual string DescPedidoStatus{ get; set; }



    }
}
