using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Util
{
    public static class ColecoesWebUtil
    {
        public static List<ClienteStatusDto> ListarClienteStatus()
        {
            return new List<ClienteStatusDto>()
                {
                   new ClienteStatusDto(){
                       CodClienteStatus = (int)ConstantesUtil.ClienteStatus.Ativo,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.ClienteStatus.Ativo)
                   },
                   new ClienteStatusDto(){
                       CodClienteStatus = (int)ConstantesUtil.ClienteStatus.Cancelado,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.ClienteStatus.Cancelado)
                   },
                   new ClienteStatusDto(){
                       CodClienteStatus = (int)ConstantesUtil.ClienteStatus.Pendente,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.ClienteStatus.Pendente)
                   }

                };
        }

        public static  List<FreteTipoDto> ListarFreteTipo()
        {
            return new List<FreteTipoDto>()
                {
                   new FreteTipoDto(){
                       CodFreteTipo = (int)ConstantesUtil.FreteTipo.PorEstado,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.FreteTipo.PorEstado)
                   },
                   new FreteTipoDto(){
                       CodFreteTipo = (int)ConstantesUtil.FreteTipo.ValorFixo,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.FreteTipo.ValorFixo)
                   }

                };
        }



        public static List<FreteEstadoDto> ListarFreteEstado()
        {
            List<FreteEstadoDto> lista = new List<FreteEstadoDto>();

            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "AC" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "AL" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "AP" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "AM" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "BA" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "CE" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "ES" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "GO" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "MA" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "MT" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "MS" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "MG" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "PA" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "PB" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "PR" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "PE" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "PI" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "RJ" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "RN" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "RS" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "RO" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "RR" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "SC" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "SP" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "SE" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "TO" });
            lista.Add(new FreteEstadoDto() { IdFreteEstado = "", UF = "DF" });

            return lista;

        }

    }
}
