using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;
using Savoia.Desencalha.Host.WebApi.Messages.Produto;
using System;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Common.Util;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class ProdutoValidator : IProdutoValidator
    {
        internal IPedidoRepository pedidoRepository;
        internal ICategoriaRepository categoriaRepository;
        internal IProdutoRepository produtoRepository;

        public ProdutoValidator(IProdutoRepository produtoRepository,
                                IPedidoRepository pedidoRepository,
                                ICategoriaRepository categoriaRepository)
        {
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
            this.categoriaRepository = categoriaRepository;
        }

        public ManterProdutoResponse Validar(ManterProdutoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterProdutoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (request.IdCategoria == null)
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.IdCategoria), Mensagem = "Selecione a categoria" });

            if (!string.IsNullOrEmpty(request.IdCategoria) && categoriaRepository.ObterAsync(request.IdCategoria).Result == null)
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.IdCategoria), Mensagem = "Categoria inexistente" });

            if (string.IsNullOrEmpty(request.CodInterno))
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.CodInterno), Mensagem = "Preencha o campo Cód Interno" });

            if (string.IsNullOrEmpty(request.Titulo))
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Titulo), Mensagem = "Preencha o campo titulo" });

            if (request.ValorVenda == null || request.ValorVenda <=0)
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.ValorVenda), Mensagem = "Preencha corretamente o campo Valor Venda" });

            if (request.Estoque == null || request.Estoque <= 0)
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.ValorVenda), Mensagem = "Preencha corretamente o campo Estoque" });

            if (request?.Imagens?.FirstOrDefault() == null)
                mensagens.Add(new MensagemSistema() { Campo = "", Mensagem = "Selecione pelo menos uma imagem " });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<ManterProdutoResponse>(mensagens);

            if (produtoRepository.ConsistirAsync(request.IdProduto, request?.IdCategoria, request.CodInterno, request.Titulo, request.IdCliente).Result)
                return GetResponseErrorValidation<ManterProdutoResponse>("", "Produto já existente");

            return new ManterProdutoResponse();
        }

        public ManterProdutoPrecoResponse Validar(ManterProdutoPrecoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterProdutoPrecoResponse>();
           
            return new ManterProdutoPrecoResponse();
        }

        public ListarProdutoResponse Validar(string codInterno, string titulo, string idCategoria = null,
                                            bool? ativo = null, bool? promocao = null, bool? lancamento = null,
                                            string ? idCliente = null)
        {
            if ((string.IsNullOrWhiteSpace(codInterno) && string.IsNullOrWhiteSpace(titulo) && string.IsNullOrEmpty(idCategoria)
                && ativo == null && promocao == null && lancamento ==null) && idCliente == null )
                return GetResponseError<ListarProdutoResponse>(MessageResource.COMUM_PESQUISA_PREENCHA_CAMPO);

            return new ListarProdutoResponse();
        }

        public ListarProdutoResponse Validar(string titulo, string idCategoria = null, string? idCliente = null)
        {
            if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrEmpty(idCategoria) && idCliente == null)
                return GetResponseError<ListarProdutoResponse>(MessageResource.COMUM_PESQUISA_PREENCHA_CAMPO);

            return new ListarProdutoResponse();
        }


        public ExcluirProdutoResponse Validar(string idProduto)
        {
            var pedido = pedidoRepository.ListarAsync(idProduto : idProduto).Result;

            if (pedido != null && pedido.Count>0)
                return GetResponseErrorValidation<ExcluirProdutoResponse>("", MessageResource.COMUM_REGISTRO_ASSOCIADO);

            return new ExcluirProdutoResponse();
        }
    }
}
