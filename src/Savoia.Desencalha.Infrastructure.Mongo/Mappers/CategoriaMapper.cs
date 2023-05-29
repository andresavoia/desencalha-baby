using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class CategoriaMapper
    {
        public static CategoriaDocument Mapper(this CategoriaModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<CategoriaModel, CategoriaDocument>(model);

            if (!string.IsNullOrEmpty(model.IdCategoria))
                retorno.IdCategoria = MongoDB.Bson.ObjectId.Parse(model.IdCategoria);

            return retorno;
        }

        public static CategoriaModel Mapper(this CategoriaDocument document)
        {
            if (document == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<CategoriaDocument, CategoriaModel>(document);

            if (document != null && document?.IdCategoria != MongoDB.Bson.ObjectId.Empty)
                retorno.IdCategoria = document.IdCategoria.ToString();

            return retorno;
        }

    }
}
