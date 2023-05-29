using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioDocument Mapper(this UsuarioModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<UsuarioModel, UsuarioDocument>(model);

            if (!string.IsNullOrEmpty(model.IdUsuario))
                retorno.IdUsuario = MongoDB.Bson.ObjectId.Parse(model.IdUsuario);

            return retorno;
        }

        public static UsuarioModel Mapper(this UsuarioDocument document)
        {
            if (document == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<UsuarioDocument, UsuarioModel>(document);

            if (document != null && document.IdUsuario != MongoDB.Bson.ObjectId.Empty)
                retorno.IdUsuario = document.IdUsuario.ToString();

            return retorno;
        }

    }
}
