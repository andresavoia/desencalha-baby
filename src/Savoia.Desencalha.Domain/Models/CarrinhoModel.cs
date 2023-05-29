using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class CarrinhoModel
    {
        public virtual string IdCliente { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime DataExpiracaoToken { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual List<PedidoItemModel> Itens { get; set; }
        public virtual DateTime DataCadastro { get; set; }
    }

}
