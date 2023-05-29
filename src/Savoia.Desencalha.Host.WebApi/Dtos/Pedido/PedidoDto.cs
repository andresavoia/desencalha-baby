using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Pedido
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoDto
    {
        [DataMember]
        public virtual int IdPedido { get; set; }
        [DataMember]
        public virtual int CodPedidoStatus { get; set; }
        [DataMember]
        public virtual string DescPedidoStatus
        {
            get
            {

                return ConstantesUtil.ObterPedidoStatus(this.CodPedidoStatus);
            }
        }

        [DataMember]
        public virtual ClienteDto Cliente { get; set; }

        [DataMember]
        public virtual double ValorTotal { get; set; }
        [DataMember]
        public virtual double ValorTotalFrete { get; set; }
        [DataMember]
        public virtual double ValorTotalComFrete { get; set; }

        [DataMember]
        public virtual string Observacao { get; set; }
        [DataMember]
        public virtual List<PedidoItemDto> Itens { get; set; }

        [DataMember]
        public virtual DateTime DataCadastro { get; set; }
        [DataMember]
        public virtual DateTime DataAlteracao { get; set; }
        [DataMember]
        public virtual string UsuarioAlteracao { get; set; }

        [DataMember]
        public virtual List<PedidoVendedorDto> PedidoVendedor { get; set; }
    }

    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoLogDto
    {
        [DataMember]
        public virtual DateTime DataAlteracao { get; set; }
        [DataMember]
        public virtual string UsuarioAlteracao { get; set; }
        [DataMember]
        public virtual int CodPedidoStatus { get; set; }
        [DataMember]
        public virtual string Observacao { get; set; }
        [DataMember]
        public virtual string DescPedidoStatus
        {
            get
            {

                return ConstantesUtil.ObterPedidoVendedorStatus(this.CodPedidoStatus);
            }
        }

    }

    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoAnexoDto
    {
        [DataMember]
        public virtual int CodTipoAnexo { get; set; }
        [DataMember]
        public virtual string Nome { get; set; }
        [DataMember]
        public virtual string Caminho { get; set; }
    }


    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoVendedorDto
    {
        [DataMember]
        public virtual string IdVendedor { get; set; }
        [DataMember]
        public virtual string NomeVendedor { get; set; }
        [DataMember]
        public virtual double ValorTotal { get; set; }
        [DataMember]
        public virtual double ValorTotalFrete { get; set; }
        [DataMember]
        public virtual double ValorTotalComFrete { get; set; }
        [DataMember]
        public virtual List<PedidoItemDto> Itens { get; set; }
        [DataMember]
        public virtual DateTime DataPrevisaoEntrega { get; set; }
        [DataMember]
        public virtual string DescFreteTipo { get; set; }

        [DataMember]
        public virtual int CodPedidoVendedorStatus { get; set; }
        [DataMember]
        public virtual string DescPedidoStatus
        {
            get
            {

                return ConstantesUtil.ObterPedidoVendedorStatus(this.CodPedidoVendedorStatus);
            }
        }
        [DataMember]
        public virtual string Observacao { get; set; }

        [DataMember]
        public virtual List<PedidoLogDto> Logs { get; set; }

        [DataMember]
        public virtual List<PedidoAnexoDto> Anexos { get; set; }

        [DataMember]
        public virtual PedidoRastreioDto Rastreio { get; set; }

    }

    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/pedido/types/")]
    public class PedidoRastreioDto
    {
        [DataMember]
        public virtual string CodigoRastreio { get; set; }
        [DataMember]
        public virtual string LinkRastreio { get; set; }
    }
}
