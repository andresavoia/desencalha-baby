using Savoia.Desencalha.Host.WebApi.Dtos.Categoria;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Produto
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/produto/types/")]
    public class ProdutoDto : BaseDto
    {
        [DataMember]
        public virtual string IdProduto { get; set; }
        [DataMember]
        public virtual string IdCategoria { get; set; }
        [DataMember]
        public virtual string IdCliente { get; set; }
        [DataMember]
        public virtual string DescCategoria{ get; set; }
        [DataMember]
        public virtual string CodInterno { get; set; }
        [DataMember]
        public virtual string Titulo { get; set; }
        [DataMember]
        public virtual int Estoque { get; set; }

        [DataMember]
        public virtual string Descricao { get; set; }
        [DataMember]
        public virtual double? ValorVenda { get; set; }
        [DataMember]
        public virtual double? Altura { get; set; }
        [DataMember]
        public virtual double? Peso { get; set; }
        public virtual double? Comprimento { get; set; }

        [DataMember]
        public virtual bool? ProdutoPromocao { get; set; }
        [DataMember]
        public virtual double? Largura { get; set; }
        [DataMember]
        public virtual bool? ProdutoLancamento { get; set; }
        [DataMember]
        public virtual string Obs { get; set; }
        [DataMember]
        public List<ProdutoPrecoLogDto> PrecosLog { get; set; }
        [DataMember]
        public List<ProdutoImagemDto> Imagens { get; set; }

        [DataMember]
        public virtual string[] RamosAtividadesDirecionado { get; set; }
    }

    public class ProdutoPrecoLogDto
    {
        [DataMember]
        public virtual DateTime DataAlteracao { get; set; }
        [DataMember]
        public virtual string UsuarioAlteracao { get; set; }
        [DataMember]
        public virtual double? ValorVenda { get; set; }
    }

    public class ProdutoImagemDto
    {
        [DataMember]
        public virtual string Nome { get; set; }
        [DataMember]
        public virtual string Caminho{ get; set; }
        [DataMember]
        public virtual bool? Principal{ get; set; }

    }


}
