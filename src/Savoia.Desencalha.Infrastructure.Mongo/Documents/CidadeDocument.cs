using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class CidadeDocument
    {
        public virtual int CodCidade { get; set; }
        public virtual EstadoDocument Estado { get; set; }
        public virtual string Titulo { get; set; }
    }
}
