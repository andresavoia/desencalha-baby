using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;
using Savoia.Desencalha.Host.WebApi.Messages.Pedido;
using System;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Dtos;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class PedidoValidator : IPedidoValidator
    {
        private IProdutoRepository produtoRepository;

        public PedidoValidator(IProdutoRepository produtoRepository)
        {
            this.produtoRepository = produtoRepository;
        }

        public ManterPedidoResponse Validar(ManterPedidoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterPedidoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if(request.IdPedido==0)
                mensagens.Add(new MensagemSistema() { Mensagem = "Id pedido nulo" });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<ManterPedidoResponse>(mensagens);

            return new ManterPedidoResponse();
        }

        public CriarPedidoResponse Validar(CriarPedidoRequest request, string idCliente)
        {
            if (request == null) return GetRequestIsNullResponse<CriarPedidoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            //Validando Endereço Entrega
            ValidarEndereco(request.EnderecoEntrega, mensagens, "EnderecoEntrega");

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<CriarPedidoResponse>(mensagens);
            
            return new CriarPedidoResponse();
        }

        private void ValidarEndereco(EnderecoDto endereco, List<MensagemSistema> mensagens, string tipoEndereco)
        {
            if (endereco == null) return;

            if (string.IsNullOrEmpty(endereco?.Endereco))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Endereco", Mensagem = "Preencha o campo Endereço" });

            if (string.IsNullOrEmpty(endereco?.Bairro))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Bairro", Mensagem = "Preencha o campo Bairro" });

            if (string.IsNullOrEmpty(endereco?.Cep))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Cep", Mensagem = "Preencha o campo CEP" });

            if (string.IsNullOrEmpty(endereco?.Cidade.Titulo) || endereco?.Cidade?.CodCidade == 0)
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Estado_Cidade_CodCidade", Mensagem = "Selecione a cidade" });

            if (string.IsNullOrEmpty(endereco?.Cidade?.Estado?.UF) || endereco?.Cidade?.Estado?.CodEstado == 0)
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Estado_CodEstado", Mensagem = "Selecione o estado" });

        }

        public ListarPedidoResponse Validar(int? idPedido = null,int ? codPedidoStatus = null, string idCliente = null, DateTime? dataCadastroInicial = null, DateTime? dataCadastroFinal = null, string cnpjCliente = null)
        {
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (idPedido == null && codPedidoStatus == null && dataCadastroInicial == null && dataCadastroFinal == null && string.IsNullOrEmpty(cnpjCliente))
                mensagens.Add(new MensagemSistema() { Campo = nameof(codPedidoStatus), Mensagem = MessageResource.COMUM_PESQUISA_PREENCHA_CAMPO});
            

            return new ListarPedidoResponse();
        }

        public ObterPedidoResponse Validar(int id)
        {
            if (id <=0) return GetRequestIsNullResponse<ObterPedidoResponse>();

            return new ObterPedidoResponse();
        }

        public ObterCarrinhoResponse ValidarCarrinho(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente)) return GetRequestIsNullResponse<ObterCarrinhoResponse>();
           
            return new ObterCarrinhoResponse();
        }

        public ManterCarrinhoResponse ValidarCarrinhoManter(ManterCarrinhoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterCarrinhoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            //if (string.IsNullOrEmpty(request.UF))
            //    mensagens.Add(new MensagemSistema() { Mensagem = "Impossivel calcular sem a UF de entrega." });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<ManterCarrinhoResponse>(mensagens);

            return new ManterCarrinhoResponse();
        }

        public CriarCarrinhoResponse ValidarCarrinhoCriar(CriarCarrinhoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<CriarCarrinhoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (request.Itens == null || request.Itens.Count == 0)
            {
                mensagens.Add(new MensagemSistema() { Mensagem = "Selecione pelo menos um produto" });
            }
            else
            {

                request.Itens.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.IdProduto))
                        mensagens.Add(new MensagemSistema() { Mensagem = "Item com idProduto em branco" });

                    if (x.Qt <= 0)
                        mensagens.Add(new MensagemSistema() { Mensagem = "Item com qt zerado" });

                    if (produtoRepository.ObterAsync(x.IdProduto).Result == null)
                        mensagens.Add(new MensagemSistema() { Campo = nameof(x.IdProduto), Mensagem = "Produto Inexistente" });

                    return;
                });


            }

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<CriarCarrinhoResponse>(mensagens);

            return new CriarCarrinhoResponse();
        }


        public ExcluirAnexoResponse Validar(ExcluirAnexoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ExcluirAnexoResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (request.IdPedido == 0)
                mensagens.Add(new MensagemSistema() { Mensagem = "Id pedido nulo" });

            if (request.Anexo == null)
                mensagens.Add(new MensagemSistema() { Mensagem = "Anexo nulo" });

            if (string.IsNullOrEmpty(request.Anexo?.Nome))
                mensagens.Add(new MensagemSistema() { Mensagem = "Nome do anexo nulo" });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<ExcluirAnexoResponse>(mensagens);

            return new ExcluirAnexoResponse();
        }

    }
}
