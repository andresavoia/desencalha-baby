
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class FreteEstadoDocument 
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId IdFreteEstado{ get; set; }
        public ObjectId IdCliente { get; set; }
        public string UF { get; set; }
        public double Valor { get; set; }
        public double? ValorPedidoFreteGratis { get; set; }

        public int? DiasPrazoEntrega { get; set; }
    }

}
