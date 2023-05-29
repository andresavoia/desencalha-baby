
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class CategoriaDocument : BaseDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId IdCategoria { get; set; }
        public string CodInterno { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}
