using Savoia.Desencalha.Host.WebApi.Dtos.Produto;
using Savoia.Desencalha.Host.WebApi.Messages.Produto;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class ProdutoMapper
    {
        public static ProdutoModel Mapper(this ManterProdutoRequest request)
        {
            if (request== null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ManterProdutoRequest,ProdutoModel>(request);
            retorno.Imgs = ColecaoUtil.AtribuirValoresLista<ProdutoImagemDto, ProdutoImagemModel>(request.Imagens);

            //retorno.RamosAtividades = request.RamosAtividades;
            return retorno;
        }

        public static ProdutoDto Mapper(this ProdutoModel model)
        {
            if (model == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ProdutoModel,ProdutoDto>(model);
            retorno.Imagens = ColecaoUtil.AtribuirValoresLista<ProdutoImagemModel, ProdutoImagemDto>(model.Imgs);
            retorno.PrecosLog = ColecaoUtil.AtribuirValoresLista<ProdutoPrecoLogModel, ProdutoPrecoLogDto>(model.PrecosLog)?.OrderByDescending(x=>x.DataAlteracao)?.ToList();

            //retorno.RamosAtividades = model.RamosAtividades;
            return retorno;
        }

        public static ProdutoModel MapperClone(this ProdutoModel model)
        {
            if (model == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ProdutoModel, ProdutoModel>(model);
            retorno.Imgs = ColecaoUtil.AtribuirValoresLista<ProdutoImagemModel, ProdutoImagemModel>(model.Imgs);
            retorno.PrecosLog = ColecaoUtil.AtribuirValoresLista<ProdutoPrecoLogModel, ProdutoPrecoLogModel>(model.PrecosLog);

            return retorno;
        }

        public static List<ProdutoDto> Mapper(this List<ProdutoModel> lista)
        {
            if (lista == null) return null;
            
            var retorno = new List<ProdutoDto>();

            lista.ForEach(x => {
                retorno.Add(x.Mapper());
            });

            return retorno;
        }
    }
}
