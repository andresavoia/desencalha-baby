using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using System;
using System.Collections.Generic;


namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class ProdutoDocument : BaseDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual ObjectId IdProduto { get; set; }
        public virtual ObjectId IdCategoria{ get; set; }
        public virtual ObjectId IdCliente { get; set; }
        public virtual string CodInterno { get; set; }
        public virtual string Titulo { get; set; }
        public virtual int Estoque { get; set; }
        public virtual string Descricao { get; set; }
        public virtual  double? ValorVenda { get; set; }
        public virtual  double? ValorVendaVendedor { get; set; }
        public virtual  double? Peso { get; set; }
        public virtual bool? ProdutoPromocao { get; set; }
        public virtual double? PercPromocao { get; set; }
        public virtual bool? ProdutoLancamento { get; set; }
        public virtual double? Altura { get; set; }
        public virtual double? Largura { get; set; }
        public virtual double? Comprimento { get; set; }
        public virtual string Obs { get; set; }
        public List<ProdutoPrecoLogDocument> PrecosLog { get; set; }
        public List<ProdutoImagemDocument> Imagens { get; set; }

        public virtual string[] RamosAtividadesDirecionado { get; set; }
    }

    public class ProdutoPrecoLogDocument
    {
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual  double? ValorVenda { get; set; }
    }

    public class ProdutoImagemDocument{
        public virtual string Nome{ get; set; }
        public virtual bool ? Principal { get; set; }
    }
}
