using System;
using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class PedidoItemModel
    {
        public virtual string IdProduto { get; set; }
        public virtual string IdCliente { get; set; }
        public virtual string Titulo { get; set; }
        public virtual double Valor { get; set; }
        public virtual int Qt{ get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual int DiasPrazoEntrega { get; set; }
    }
}
