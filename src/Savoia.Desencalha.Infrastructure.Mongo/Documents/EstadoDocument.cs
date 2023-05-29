using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class EstadoDocument
    {
        public virtual int CodEstado { get; set; }
        public virtual string UF { get; set; }
        public virtual string Titulo { get; set; }
    }
}
