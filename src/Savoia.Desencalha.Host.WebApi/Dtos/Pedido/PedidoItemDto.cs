using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoItemDto 
    {
        [DataMember]
        public virtual string IdProduto { get; set; }
        [DataMember]
        public virtual string IdCliente { get; set; }
        [DataMember]
        public virtual string CaminhoImagemPrincipal { get; set; }
        [DataMember]
        public virtual string Titulo { get; set; }
        [DataMember]
        public virtual double Valor { get; set; }
        [DataMember]
        public virtual int Qt { get; set; }
        [DataMember]
        public virtual double ValorTotal { get; set; }
        [DataMember]
        public virtual double? Largura { get; set; }
        [DataMember]
        public virtual int EstoqueDisponivel { get; set; }
        [DataMember]
        public virtual int DiasPrazoEntrega { get; set; }

    }
}
