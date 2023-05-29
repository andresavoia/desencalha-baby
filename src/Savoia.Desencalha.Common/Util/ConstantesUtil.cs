using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Savoia.Desencalha.Common.Util
{
    public class ConstantesUtil
    {
        public const int PAGE_SIZE_DEFAULT = 500;
        public const int TIMEOUT_SESSAO_MINUTOS = 120;
        public const string PESSOA_TIPO_FISICA = "PF";
        public const string PESSOA_TIPO_JURIDICA = "PJ";
        public const string USUARIO_TIPO_ADM = "ADM";
        public const string USUARIO_TIPO_CLI_ADM = "CLI_ADM";
        public const string PRODUTO_IMAGEM_TIPO_P = "P";
        public const string PRODUTO_IMAGEM_TIPO_G = "G";

        #region -- Mensagens de observação do sistema --
        public const string PEDIDO_MULTIPLOS_VENDEDORES = "Pedido com múltiplos vendedores! Você receberá um boleto de cobrança por cada vendedor no pedido.";
        #endregion

        public enum Status
        {
            [Description("Ativo")]
            Ativo = 1,
            [Description("Inativo")]
            Inativo = 0,
        }

        public enum ClienteStatus {
            [Description("Pendente")]
            Pendente = 1,
            [Description("Ativo")]
            Ativo = 2,
            [Description("Cancelado")]
            Cancelado = 3
        }

        public enum PedidoVendedorStatus
        {
            [Description("Pendente")]
            Pendente = 1,
            [Description("Aguardando Pagamento")]
            AguardandoPagto = 2,
            [Description("Nota Fiscal Gerada")]
            NotaFiscalGerada = 3,
            [Description("Cancelado")]
            Cancelado = 4,
            [Description("Pedido Enviado")]
            PedidoEnviado = 5,
            [Description("Entregue")]
            Entregue = 6
        }

        public enum PedidoStatus
        {
            [Description("Em andamento")]
            EmAndamento = 1,
            [Description("Finalizado")]
            Finalizado = 2,
            [Description("Cancelado")]
            Cancelado = 3
        }

        public enum FreteTipo
        {
            [Description("Por Estado")]
            PorEstado = 1,
            [Description("Valor Fixo")]
            ValorFixo = 2
        }

        public enum PedidoAnexo
        {
            [Description("Boleto")]
            Boleto = 1,
            [Description("Nota Fiscal")]
            NotaFiscal = 2
        }

        public static string ObterPedidoStatus(int codPedidoStatus)
        {
            if (codPedidoStatus == (int)PedidoStatus.EmAndamento)
                return GetEnumDescription(PedidoStatus.EmAndamento);
            else if (codPedidoStatus == (int)PedidoStatus.Finalizado)
                return GetEnumDescription(PedidoStatus.Finalizado);
            else if (codPedidoStatus == (int)PedidoStatus.Cancelado)
                return GetEnumDescription(PedidoStatus.Cancelado);
            else
                throw new Exception("ConstantesUtil.ObterPedidoStatus não tem esse codigopedido status");
        }

        public static string ObterPedidoVendedorStatus(int codPedidoStatus)
        {
            if (codPedidoStatus == (int)PedidoVendedorStatus.Pendente)
                return GetEnumDescription(PedidoVendedorStatus.Pendente);
            else if (codPedidoStatus == (int)PedidoVendedorStatus.AguardandoPagto)
                return GetEnumDescription(PedidoVendedorStatus.AguardandoPagto);
            else if (codPedidoStatus == (int)PedidoVendedorStatus.NotaFiscalGerada)
                return GetEnumDescription(PedidoVendedorStatus.NotaFiscalGerada);
            else if (codPedidoStatus == (int)PedidoVendedorStatus.PedidoEnviado)
                return GetEnumDescription(PedidoVendedorStatus.PedidoEnviado);
            else if (codPedidoStatus == (int)PedidoVendedorStatus.Entregue)
                return GetEnumDescription(PedidoVendedorStatus.Entregue);
            else if (codPedidoStatus == (int)PedidoVendedorStatus.Cancelado)
                return GetEnumDescription(PedidoVendedorStatus.Cancelado);
            else
                throw new Exception("ConstantesUtil.ObterPedidoStatus não tem esse codigopedido status");
        }



        public static string ObterFreteTipo(int codFreteTipo)
        {
            if (codFreteTipo== (int)FreteTipo.PorEstado)
                return GetEnumDescription(FreteTipo.PorEstado);
            else if (codFreteTipo == (int)FreteTipo.ValorFixo)
                return GetEnumDescription(FreteTipo.ValorFixo);
            else
                throw new Exception("ConstantesUtil.ObterFreteTipo não tem esse fretetipo");
        }


        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

    }



}
