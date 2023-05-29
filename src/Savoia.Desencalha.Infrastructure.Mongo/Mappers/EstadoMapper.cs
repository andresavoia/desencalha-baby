using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class EstadoMapper
    {
        public static EstadoDocument Mapper(this EstadoModel model)
        {

            var retorno = ColecaoUtil.AtribuirValores<EstadoModel, EstadoDocument>(model);
            
            return retorno;
        }

        public static EstadoModel Mapper(this EstadoDocument document)
        {
            var retorno = ColecaoUtil.AtribuirValores<EstadoDocument, EstadoModel>(document);

            return retorno;
        }

    }
}
