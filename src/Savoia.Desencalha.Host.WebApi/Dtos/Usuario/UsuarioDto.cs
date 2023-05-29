using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Usuario
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/usuario/types/")]
    public class UsuarioDto
    {

        [DataMember]
        public string IdUsuario { get; set; }
        [DataMember]
        public string CodUsuarioTipo { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Cpf { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public bool Ativo { get; set; }
    }
}
