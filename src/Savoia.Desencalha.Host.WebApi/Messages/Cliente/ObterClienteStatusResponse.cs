using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.ClienteStatus
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/ClienteStatus/types/")]
    public class ObterClienteStatusResponse : BaseResponse
    {
        [DataMember]
        public ClienteStatusDto Dados { get; set;}
    }
}
