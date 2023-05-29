using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.PedidoStatus
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/PedidoStatus/types/")]
    public class ObterPedidoStatusResponse : BaseResponse
    {
        [DataMember]
        public PedidoStatusDto Dados { get; set;}
    }
}
