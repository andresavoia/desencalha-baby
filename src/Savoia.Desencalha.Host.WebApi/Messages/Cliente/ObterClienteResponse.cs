﻿using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Cliente/types/")]
    public class ObterClienteResponse : BaseResponse
    {
        [DataMember]
        public ClienteDto Dados { get; set;}
    }
}
