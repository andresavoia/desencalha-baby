using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.RamoAtividade
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/ramoatividade/types/")]
    public class RamoAtividadeDto : BaseDto
    {
        [DataMember]
        public string IdRamoAtividade { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public string Descricao { get; set; }
    }
}
