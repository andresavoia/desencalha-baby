using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/cliente/types/")]
    public class ExcluirClienteResponse : BaseResponse
    {
        [DataMember]
        public long? RegistrosExcluidos { get; set;}
    }
}
