using Savoia.Desencalha.Host.WebApi.Dtos.RamoAtividade;
using Savoia.Desencalha.Host.WebApi.Messages.RamoAtividade;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class RamoAtividadeMapper
    {
        public static RamoAtividadeModel Mapper(this ManterRamoAtividadeRequest request)
        {
            return ColecaoUtil.AtribuirValores<ManterRamoAtividadeRequest,RamoAtividadeModel>(request);
        }

        public static RamoAtividadeDto Mapper(this RamoAtividadeModel model)
        {
            return ColecaoUtil.AtribuirValores<RamoAtividadeModel,RamoAtividadeDto>(model);
        }

        public static List<RamoAtividadeDto> Mapper(this List<RamoAtividadeModel> lista)
        {
            return ColecaoUtil.AtribuirValoresLista<RamoAtividadeModel, RamoAtividadeDto>(lista);
        }
    }
}
