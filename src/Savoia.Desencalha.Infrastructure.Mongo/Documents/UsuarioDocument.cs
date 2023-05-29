
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class UsuarioDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId IdUsuario { get; set; }
        public string CodUsuarioTipo { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Login { get; set; } 
        public string Senha { get; set; }
        public string Salt { get; set; }
        public bool Ativo { get; set; }
    }
}
