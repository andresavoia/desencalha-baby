using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class RamoAtividadeMapper
    {
        public static RamoAtividadeDocument Mapper(this RamoAtividadeModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<RamoAtividadeModel, RamoAtividadeDocument>(model);

            if (!string.IsNullOrEmpty(model.IdRamoAtividade))
                retorno.IdRamoAtividade = MongoDB.Bson.ObjectId.Parse(model.IdRamoAtividade);

            return retorno;
        }

        public static RamoAtividadeModel Mapper(this RamoAtividadeDocument document)
        {
            if (document == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<RamoAtividadeDocument, RamoAtividadeModel>(document);

            if (document != null && document?.IdRamoAtividade != MongoDB.Bson.ObjectId.Empty)
                retorno.IdRamoAtividade = document.IdRamoAtividade.ToString();

            return retorno;
        }

    }
}
