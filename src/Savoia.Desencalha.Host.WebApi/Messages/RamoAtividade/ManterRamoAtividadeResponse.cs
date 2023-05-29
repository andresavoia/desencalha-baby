using Savoia.Desencalha.Host.WebApi.Dtos.RamoAtividade;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.RamoAtividade
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/RamoAtividade/types/")]
    public class ManterRamoAtividadeResponse : BaseResponse
    {
        [DataMember]
        public virtual string Id { get; set; }
    }
}
