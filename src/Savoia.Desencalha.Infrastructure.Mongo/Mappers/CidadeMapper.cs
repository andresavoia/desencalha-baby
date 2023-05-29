using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class CidadeMapper
    {
        public static CidadeDocument Mapper(this CidadeModel model)
        {

            var retorno = ColecaoUtil.AtribuirValores<CidadeModel, CidadeDocument>(model);
            
            return retorno;
        }

        public static CidadeModel Mapper(this CidadeDocument document)
        {
            var retorno = ColecaoUtil.AtribuirValores<CidadeDocument, CidadeModel>(document);

            return retorno;
        }

    }
}
