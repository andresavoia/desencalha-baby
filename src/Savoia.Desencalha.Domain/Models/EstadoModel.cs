using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Domain.Models
{
    public class EstadoModel
    {
        public virtual int CodEstado { get; set; }
        public virtual string UF { get; set; }
        public virtual string Titulo { get; set; }
    }
}
