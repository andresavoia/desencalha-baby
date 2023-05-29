using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioModel Mapper(this ManterUsuarioRequest request)
        {
            return ColecaoUtil.AtribuirValores<ManterUsuarioRequest,UsuarioModel>(request);
        }

        public static UsuarioDto Mapper(this UsuarioModel model)
        {
            return ColecaoUtil.AtribuirValores<UsuarioModel,UsuarioDto>(model);
        }

        public static List<UsuarioDto> Mapper(this List<UsuarioModel> lista)
        {
            return ColecaoUtil.AtribuirValoresLista<UsuarioModel, UsuarioDto>(lista);
        }
    }
}
