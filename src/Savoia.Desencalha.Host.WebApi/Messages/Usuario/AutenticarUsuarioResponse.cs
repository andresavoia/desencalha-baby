using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Usuario
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Usuario/types/")]
    public class AutenticarUsuarioResponse : BaseResponse
    {
        [DataMember]
        public virtual UsuarioSessaoDto Dados { get; set; }

    }
}
