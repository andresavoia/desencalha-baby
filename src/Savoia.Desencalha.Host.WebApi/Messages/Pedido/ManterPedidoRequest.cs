using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Host.WebApi.Messages.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Pedido/types/")]
    public class ManterPedidoRequest
    {
        [DataMember]
        public virtual int IdPedido { get; set; }
        [DataMember]
        public virtual int CodPedidoStatus { get; set; }
        [DataMember]
        public virtual List<PedidoAnexoDto> Anexos { get; set; }
        [DataMember]
        public virtual string Observacao{ get; set; }
        [DataMember]
        public virtual string IdVendedor { get; set; } //usado qd abre o pedido via admininistrador
        [DataMember]
        public virtual string CodigoRastreio { get; set; } 
        [DataMember]
        public virtual string LinkRastreio { get; set; } 
    }
}
