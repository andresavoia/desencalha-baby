using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/cliente/types/")]
    public class ClienteSessaoDto
    {
        [DataMember]
        public string IdCliente { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string CodUsuarioTipo { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string CEP { get; set; }
        [DataMember]
        public List<string> Categorias { get; set; }
        [DataMember]
        public bool? IsVendedor { get; set; }

    }
}
