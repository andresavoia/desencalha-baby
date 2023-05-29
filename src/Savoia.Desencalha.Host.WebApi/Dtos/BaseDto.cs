using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Dtos
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/dto-base/types/")]
    public class BaseDto
    {
        [DataMember]
        public string UsuarioCadastro { get; set; }
        [DataMember]
        public string UsuarioAlteracao { get; set; }
        [DataMember]
        public DateTime? DataCadastro { get; set; }
        [DataMember]
        public DateTime? DataAlteracao { get; set; }
        [DataMember]
        public bool Ativo { get; set; }
    }
}
