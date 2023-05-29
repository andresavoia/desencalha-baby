using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/clientestatus/types/")]
    public class ClienteStatusDto 
    {
        [DataMember]
        public int? CodClienteStatus { get; set; }
        [DataMember]
        public string Titulo { get; set; }
    }
}
