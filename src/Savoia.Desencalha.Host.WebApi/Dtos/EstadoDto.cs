using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Dtos
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/estado/types/")]
    public class EstadoDto
    {
        [DataMember]
        public int CodEstado { get; set; }
        [DataMember]
        public string UF { get; set; }
        [DataMember]
        public string Titulo { get; set; }

    }
}