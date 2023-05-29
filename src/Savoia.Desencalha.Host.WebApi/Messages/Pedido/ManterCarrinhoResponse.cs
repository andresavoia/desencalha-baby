using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class ManterCarrinhoResponse : BaseResponse
    {
        [DataMember]
        public virtual CarrinhoDto Dados { get;set; }
    }
}
