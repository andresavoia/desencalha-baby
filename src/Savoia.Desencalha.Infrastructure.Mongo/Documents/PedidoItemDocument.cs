using System;


namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class PedidoItemDocument
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
