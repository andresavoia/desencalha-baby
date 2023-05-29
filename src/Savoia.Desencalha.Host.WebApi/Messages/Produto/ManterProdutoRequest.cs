using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Produto;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Savoia.Desencalha.Host.WebApi.Dtos.Categoria;

namespace Savoia.Desencalha.Host.WebApi.Messages.Produto
{ 
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Produto/types/")]
    public class ManterProdutoRequest
    {
        [DataMember]
        public virtual string IdProduto { get; set; }
        [DataMember]
        public virtual string IdCategoria { get; set; }
        [DataMember]
        public virtual string IdCliente { get; set; }
        [DataMember]
        public virtual string CodInterno { get; set; }
        [DataMember]
        public virtual string Titulo { get; set; }
        [DataMember]
        public virtual int? Estoque { get; set; }
        [DataMember]
        public virtual string Descricao { get; set; }
        [DataMember]
        public virtual double? ValorVenda { get; set; }
        [DataMember]
        public virtual double? Altura { get; set; }
        [DataMember]
        public virtual double? Peso { get; set; }
        [DataMember]
        public virtual bool? ProdutoPromocao { get; set; }
        [DataMember]
        public virtual double? Largura { get; set; }
        [DataMember]
        public virtual double? Comprimento { get; set; }

        [DataMember]
        public virtual bool? ProdutoLancamento { get; set; }
        [DataMember]
        public virtual bool Ativo { get; set; }
        [DataMember]
        public virtual string Obs { get; set; }
        [DataMember]
        public List<ProdutoImagemDto> Imagens { get; set; }
        [DataMember]
        public virtual string[] RamosAtividadesDirecionado { get; set; }
    }
}
