using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class ProdutoModel : BaseModel
    {
        public virtual string IdProduto { get; set; }
        public virtual string IdCategoria { get; set; }
        public virtual string IdCliente {get;set;}
        public virtual string CodInterno { get; set; }
        public virtual string Titulo { get; set; }
        public virtual int Estoque { get; set; }
        public virtual string Descricao { get; set; }
        public virtual  double? ValorVenda { get; set; }
        public virtual  double? Peso { get; set; }
        public virtual bool? ProdutoPromocao { get; set; }
        public virtual double? Largura { get; set; }
        public virtual double? Comprimento { get; set; }
        public virtual double? Altura { get; set; }
        public virtual bool? ProdutoLancamento { get; set; }
        public virtual string Obs { get; set; }
        public List<ProdutoPrecoLogModel> PrecosLog { get; set; }
        public List<ProdutoImagemModel> Imgs { get; set; }

        public virtual string[] RamosAtividadesDirecionado { get; set; }

        public virtual long TotalRegistros { get; set; } //gambi para retornar o total de registros de produtos.. não retorna para camada de Service

    }

    public class ProdutoPrecoLogModel
    {
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual  double? ValorVenda { get; set; }
    }

    public class ProdutoImagemModel{
        public virtual string Nome{ get; set; }
        public virtual bool ? Principal{ get; set; }
    }
}
