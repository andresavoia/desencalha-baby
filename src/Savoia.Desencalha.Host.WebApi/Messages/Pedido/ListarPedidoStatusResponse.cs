using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/PedidoStatus/types/")]
    public class ListarPedidoStatusResponse : BaseResponse
    {
        [DataMember]
        public List<PedidoStatusDto> Dados { get; set;}
    }
}
