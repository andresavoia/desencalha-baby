using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Cliente/types/")]
    public class ListarClienteStatusResponse : BaseResponse
    {
        [DataMember]
        public List<ClienteStatusDto> Dados { get; set;}
    }
}
