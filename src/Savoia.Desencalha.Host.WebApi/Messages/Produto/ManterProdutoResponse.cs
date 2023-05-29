using Savoia.Desencalha.Host.WebApi.Dtos.Produto;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Produto
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Produto/types/")]
    public class ManterProdutoResponse : BaseResponse
    {
        [DataMember]
        public virtual string Id { get; set; }
    }
}
