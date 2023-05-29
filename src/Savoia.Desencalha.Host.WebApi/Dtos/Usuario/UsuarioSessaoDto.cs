using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Usuario
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/usuario/types/")]
    public class UsuarioSessaoDto
    {
        [DataMember]
        public string IdUsuario { get; set; }
        [DataMember]
        public string CodUsuarioTipo { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Token { get; set; }

    }
}
