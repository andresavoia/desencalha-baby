using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Dtos
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/endereco/types/")]
    public class EnderecoDto
    {
        [DataMember]
        public string IdEndereco { get; set; }
        [DataMember]
        public CidadeDto Cidade { get; set; }
        [DataMember]
        public string Bairro { get; set; }
        [DataMember]
        public string Endereco { get; set; }
        [DataMember]
        public string Complemento { get; set; }
        [DataMember]
        public string Cep { get; set; }
        [DataMember]
        public string Obs { get; set; }

    }
}
