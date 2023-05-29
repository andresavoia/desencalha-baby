
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class RamoAtividadeDocument : BaseDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId IdRamoAtividade { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}
