using Savoia.Desencalha.Host.WebApi.Messages.Categoria;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class CategoriaValidator : ICategoriaValidator
    {
        internal ICategoriaRepository usuarioRepository;
        internal IProdutoRepository produtoRepository;
        
        public CategoriaValidator(ICategoriaRepository usuarioRepository,
                                    IProdutoRepository produtoRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.produtoRepository = produtoRepository;
        }

        public ManterCategoriaResponse Validar(ManterCategoriaRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterCategoriaResponse>();

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                return GetResponseErrorValidation<ManterCategoriaResponse>(nameof(request.Titulo),"Preencha o campo titulo");
            }

            if (usuarioRepository.ConsistirAsync(request.IdCategoria,request.CodInterno, request.Titulo).Result)
                return GetResponseErrorValidation<ManterCategoriaResponse>("",MessageResource.COMUM_REGISTRO_EXISTENTE);

            return new ManterCategoriaResponse();
        }

        public ListarCategoriaResponse Validar(string codInterno, string titulo, bool? ativo)
        {
            //if (string.IsNullOrWhiteSpace(codInterno) && string.IsNullOrWhiteSpace(titulo) && ativo ==null)
            //    return GetResponseError<ListarCategoriaResponse>(MessageResource.COMUM_PESQUISA_PREENCHA_CAMPO);

            return new ListarCategoriaResponse();
        }

        public ExcluirCategoriaResponse Validar(string idCategoria)
        {
            var produto = produtoRepository.ListarAsync(null, null, idCategoria: idCategoria)?.Result?.FirstOrDefault();

            if (produto!=null && !string.IsNullOrEmpty(produto.IdProduto))
                return GetResponseErrorValidation<ExcluirCategoriaResponse>("",MessageResource.COMUM_REGISTRO_ASSOCIADO);

            return new ExcluirCategoriaResponse();
        }
    }
}
