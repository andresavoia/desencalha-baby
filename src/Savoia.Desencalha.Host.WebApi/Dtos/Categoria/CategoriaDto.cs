using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Categoria
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/categoria/types/")]
    public class CategoriaDto : BaseDto
    {
        [DataMember]
        public string IdCategoria { get; set; }
        [DataMember]
        public string CodInterno { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public bool SelecionadaClienteLogado { get; set; }

    }
}
