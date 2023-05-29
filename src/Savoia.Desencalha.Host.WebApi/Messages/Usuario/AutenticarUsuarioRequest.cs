using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Usuario
{ 
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Usuario/types/")]
    public class AutenticarUsuarioRequest
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Senha { get; set; }
    }
}
