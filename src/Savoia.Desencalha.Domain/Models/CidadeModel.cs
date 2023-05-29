using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Domain.Models
{
    public class CidadeModel
    {
        public virtual int CodCidade { get; set; }
        public virtual EstadoModel Estado { get; set; }
        public virtual string Titulo { get; set; }
    }
}
