using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Cliente
{ 
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Cliente/types/")]
    public class EnviarFaleConoscoRequest
    {
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Empresa { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Celular { get; set; }
        [DataMember]
        public string Telefone { get; set; }
        [DataMember]
        public string Observacao{ get; set; }
    }
}
