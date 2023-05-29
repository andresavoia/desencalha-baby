using Savoia.Desencalha.Host.WebApi.Dtos.Categoria;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Frete
{ 
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/frete/types/")]
    public class ManterFreteEstadoRequest
    {
        [DataMember]
        public List<FreteEstadoDto> Fretes { get; set; }

        [DataMember]
        public string IdCliente { get; set; }

    }
}
