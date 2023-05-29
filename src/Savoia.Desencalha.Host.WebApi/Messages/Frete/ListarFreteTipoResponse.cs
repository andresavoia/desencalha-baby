using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Frete
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/frete/types/")]
    public class ListarFreteTipoResponse : BaseResponse
    {
        [DataMember]
        public List<FreteTipoDto> Dados { get; set;}
    }
}
