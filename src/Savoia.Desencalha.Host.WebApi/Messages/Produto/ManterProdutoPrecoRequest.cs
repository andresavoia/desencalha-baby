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
    public class ManterProdutoPrecoRequest
    {
        [DataMember]
        public virtual List<ProdutoPrecoItem> Produtos { get; set; }
    }

    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Produto/types/")]
    public class ProdutoPrecoItem
    {
        [DataMember]
        public virtual string IdProduto { get; set; }
        [DataMember]
        public virtual double? ValorVenda { get; set; }

    }

}
