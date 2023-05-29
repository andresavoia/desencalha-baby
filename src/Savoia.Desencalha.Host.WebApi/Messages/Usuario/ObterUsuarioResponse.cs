﻿using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Usuario
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/usuario/types/")]
    public class ObterUsuarioResponse : BaseResponse
    {
        [DataMember]
        public UsuarioDto Dados { get; set;}
    }
}
