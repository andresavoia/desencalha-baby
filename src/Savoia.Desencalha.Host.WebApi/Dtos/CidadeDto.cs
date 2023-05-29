using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Dtos
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/cidade/types/")]
    public class CidadeDto
    {
        [DataMember]
        public int CodCidade { get; set; }
        [DataMember]
        public EstadoDto Estado { get; set; }
        [DataMember]
        public string Titulo { get; set; }

    }
}