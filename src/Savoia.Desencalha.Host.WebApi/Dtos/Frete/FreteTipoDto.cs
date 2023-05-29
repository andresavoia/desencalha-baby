using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Frete
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/frete/types/")]
    public class FreteTipoDto 
    {
        [DataMember]
        public int ? CodFreteTipo { get; set; }
        [DataMember]
        public string Titulo { get; set; }
    }
}
