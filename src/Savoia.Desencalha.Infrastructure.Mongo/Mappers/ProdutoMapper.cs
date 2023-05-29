using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using System.Collections.Generic;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class ProdutoMapper
    {
        public static ProdutoDocument Mapper(this ProdutoModel model)
        {
            if (model == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ProdutoModel, ProdutoDocument>(model);

            if (model.Imgs != null)
                retorno.Imagens = new List<ProdutoImagemDocument>();
            
            retorno.PrecosLog = ColecaoUtil.AtribuirValoresLista<ProdutoPrecoLogModel, ProdutoPrecoLogDocument>(model.PrecosLog);
            retorno.Imagens = ColecaoUtil.AtribuirValoresLista<ProdutoImagemModel, ProdutoImagemDocument>(model.Imgs);


            if (!string.IsNullOrEmpty(model.IdProduto))
                retorno.IdProduto = MongoDB.Bson.ObjectId.Parse(model.IdProduto);

            if (!string.IsNullOrEmpty(model.IdCategoria))
                retorno.IdCategoria = MongoDB.Bson.ObjectId.Parse(model.IdCategoria);

            if (!string.IsNullOrEmpty(model.IdCliente))
                retorno.IdCliente = MongoDB.Bson.ObjectId.Parse(model.IdCliente);

            return retorno;
        }

        public static ProdutoModel Mapper(this ProdutoDocument document)
        {
            if (document == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ProdutoDocument, ProdutoModel>(document);

            retorno.PrecosLog = ColecaoUtil.AtribuirValoresLista<ProdutoPrecoLogDocument,ProdutoPrecoLogModel>(document.PrecosLog);
            retorno.Imgs = ColecaoUtil.AtribuirValoresLista<ProdutoImagemDocument,ProdutoImagemModel>(document.Imagens);
            
            if (document!=null && document.IdProduto != MongoDB.Bson.ObjectId.Empty)
                retorno.IdProduto = document.IdProduto.ToString();

            if (document != null && document.IdCategoria != MongoDB.Bson.ObjectId.Empty)
                retorno.IdCategoria = document.IdCategoria.ToString();

            if (document != null && document.IdCliente != MongoDB.Bson.ObjectId.Empty)
                retorno.IdCliente = document.IdCliente.ToString();

            return retorno;
        }

    }
}
