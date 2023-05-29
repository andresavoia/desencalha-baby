using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class CarrinhoDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id{ get; set; }
        public virtual ObjectId IdCliente { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime DataExpiracaoToken { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual List<PedidoItemDocument> Itens { get; set; }
    }

}
