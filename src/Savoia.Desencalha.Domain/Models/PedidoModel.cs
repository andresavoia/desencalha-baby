using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Savoia.Desencalha.Domain.Models
{
    public class PedidoModel
    {
        public virtual int IdPedido { get; set; }
        public virtual int CodPedidoStatus { get; set; }
        public virtual ClienteModel Cliente { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual DateTime DataCadastro { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual string Observacao { get; set; }
    }

    public class PedidoLogModel
    {
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual int CodPedidoStatus{ get; set; }
        public virtual string Observacao{ get; set; }
    }

    public class PedidoAnexoModel
    {
        public virtual int CodTipoAnexo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Caminho { get; set; }

    }

    public class PedidoVendedorModel
    {
        public virtual string IdPedidoVendedor { get; set; }
        public virtual string IdCliente { get; set; }
        public virtual int IdPedido { get; set; }
        public virtual int CodFreteTipoUtilizado { get; set; }
        public virtual int CodPedidoVendedorStatus { get; set; }
        public virtual double ValorTotal { get; set; }
        public virtual double ValorTotalFrete { get; set; }
        public virtual double ValorTotalComFrete { get; set; }
        public virtual List<PedidoItemModel> Itens { get; set; }
        public virtual List<PedidoLogModel> Logs { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual DateTime DataPrevisaoEntrega { get; set; }
        public virtual List<PedidoAnexoModel> Anexos { get; set; }
        public virtual string Observacao { get; set; }
        public virtual PedidoRastreioModel Rastreio { get; set; }

    }

    public class PedidoRastreioModel
    {
        public virtual string CodigoRastreio { get; set; }
        public virtual string LinkRastreio { get; set; }
    }
}
