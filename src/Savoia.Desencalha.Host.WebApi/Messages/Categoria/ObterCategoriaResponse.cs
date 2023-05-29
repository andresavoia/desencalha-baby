using Savoia.Desencalha.Host.WebApi.Dtos.Categoria;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Categoria
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Categoria/types/")]
    public class ObterCategoriaResponse : BaseResponse
    {
        [DataMember]
        public CategoriaDto Dados { get; set;}
    }
}
