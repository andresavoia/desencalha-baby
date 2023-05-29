using Savoia.Desencalha.Host.WebApi.Dtos.Categoria;
using Savoia.Desencalha.Host.WebApi.Messages.Categoria;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class CategoriaMapper
    {
        public static CategoriaModel Mapper(this ManterCategoriaRequest request)
        {
            return ColecaoUtil.AtribuirValores<ManterCategoriaRequest,CategoriaModel>(request);
        }

        public static CategoriaDto Mapper(this CategoriaModel model)
        {
            return ColecaoUtil.AtribuirValores<CategoriaModel,CategoriaDto>(model);
        }

        public static List<CategoriaDto> Mapper(this List<CategoriaModel> lista)
        {
            return ColecaoUtil.AtribuirValoresLista<CategoriaModel, CategoriaDto>(lista);
        }
    }
}
