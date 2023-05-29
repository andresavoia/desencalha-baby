using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Cliente
{
    public class ClienteAlterarSenhaDto
    {
        public string IdCliente { get; set; }
        public DateTime DataAutorizacao { get; set; }
    }
}
