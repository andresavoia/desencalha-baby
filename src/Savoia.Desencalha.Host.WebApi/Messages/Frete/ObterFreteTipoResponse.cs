using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Frete
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/frete/types/")]
    public class ObterFreteTipoResponse : BaseResponse
    {
        [DataMember]
        public FreteTipoDto Dados { get; set;}
    }
}
