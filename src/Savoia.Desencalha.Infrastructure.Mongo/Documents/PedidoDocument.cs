using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class PedidoDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.Int32)]
        public virtual int IdPedido { get; set; }
        public virtual int CodPedidoStatus { get; set; }
        public virtual ClienteDocument Cliente { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual DateTime DataCadastro { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual string Observacao { get; set; }
    }

    public class PedidoLogDocument
    {
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual int CodPedidoStatus{ get; set; }
        public virtual string Observacao{ get; set; }
    }

    public class PedidoAnexoDocument
    {
        public virtual int CodTipoAnexo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Caminho { get; set; }

    }

    public class PedidoVendedorDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual ObjectId IdPedidoVendedor { get; set; }
        public virtual string IdCliente { get; set; }
        public virtual int IdPedido { get; set; }
        public virtual int CodFreteTipoUtilizado { get; set; }
        public virtual int CodPedidoVendedorStatus { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual List<PedidoItemDocument> Itens { get; set; }
        public virtual List<PedidoLogDocument> Logs { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual DateTime DataPrevisaoEntrega { get; set; }
        public virtual List<PedidoAnexoDocument> Anexos { get; set; }
        public virtual string Observacao { get; set; }
        public virtual PedidoRastreioDocument Rastreio { get; set; }
    }

    public class PedidoRastreioDocument
    {
        public virtual string CodigoRastreio { get; set; }
        public virtual string LinkRastreio { get; set; }
    }

}
