using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Savoia.Desencalha.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using Newtonsoft.Json;
using Savoia.Desencalha.Common.Util;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;

namespace Savoia.Desencalha.Host.WebApi.Filters
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    //public class AutorizacaoFilter : ActionFilterAttribute
    //{
    //    private IPedidoRepository pedidoRepository;
    //    private IConfiguration configuration;
    //    private IClienteRepository clienteRepository;

    //    public string[] PerfisAcesso { get; set; }
    //    public bool NaoLancarNaoAutorizado { get; set; }

    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        var token = context.HttpContext?.Request?.Headers["Authorization"];

    //        this.pedidoRepository = context.HttpContext.RequestServices.GetService<IPedidoRepository>();
    //        this.configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

    //        var resultado = false;
    //        this.clienteRepository = context.HttpContext.RequestServices.GetService<IClienteRepository>();

    //        //primeira coisa, é verificar se o metodo tem liberado para cliente
    //        if ((PerfisAcesso.ToList()?.Where(x => x == ConstantesUtil.USUARIO_TIPO_ADM)?.Any() == false))
    //        {
    //            if (!NaoLancarNaoAutorizado)
    //                context.Result = new UnauthorizedResult();
    //        }
    //        else
    //        {
    //            var carrinho = pedidoRepository.ObterCarrinhoPorTokenAsync(token).Result;

    //            if (carrinho?.DataExpiracaoToken > DateTime.Now)
    //                resultado = pedidoRepository.AlterarValidadeCarrinhoAsync(token).Result;
    //            else
    //                resultado = false;

    //            if (!resultado)
    //            {
    //                if (!NaoLancarNaoAutorizado)
    //                    context.Result = new UnauthorizedResult();
    //            }
    //            else
    //            {
    //                //pegando os dados do cliente para jogar na sessao
    //                var cliente = clienteRepository.ObterAsync(carrinho.IdCliente).Result;

    //                var clienteSessao = new ClienteSessaoDto()
    //                {
    //                    IdCliente = carrinho.IdCliente,
    //                    Nome = cliente?.NomeOuRazao
    //                };

    //                //GRAVANDO O USUARIO NO COOKIE
    //                if (!context.HttpContext.Request.Headers[ConstantesWebUtil.CLIENTE_SESSAO].Any())
    //                    context.HttpContext.Request.Headers.Add(new KeyValuePair<string, StringValues>(ConstantesWebUtil.CLIENTE_SESSAO, CriptografiaUtil.EncriptarPadrao(JsonConvert.SerializeObject(clienteSessao))));

    //            }
    //        }

           
    //        base.OnActionExecuting(context);
    //    }
    //}

    public class Token {
        public string authorization { get; set; }
    }

    public class DadosRetorno
    {
        #region Construtores

        public DadosRetorno() { }

        public DadosRetorno(string Mensagem, bool Sucesso)
            : this(Mensagem, Sucesso, false, null)
        {
        }

        public DadosRetorno(string Mensagem, bool Sucesso, bool PossuiRetorno)
            : this(Mensagem, Sucesso, PossuiRetorno, null)
        {
        }

        public DadosRetorno(string Mensagem, bool Sucesso, bool PossuiRetorno, UsuarioSessaoDto Dados)
        {
            this._Dados = Dados;
            this._PossuiRetorno = PossuiRetorno;
            this._Sucesso = Sucesso;
        }

        #endregion

        #region Variáveis
        private bool _Sucesso = true;
        private bool _PossuiRetorno;
        private UsuarioSessaoDto _Dados;
        private List<ValidationResult> _Mensagens;

        #endregion

        #region Propriedades


        public bool Sucesso
        {
            get { return this._Sucesso; }
            set { this._Sucesso = value; }
        }

        public bool PossuiRetorno
        {
            get { return this._PossuiRetorno; }
            set { this._PossuiRetorno = value; }
        }


        public UsuarioSessaoDto Dados
        {
            get { return this._Dados; }
            set
            {
                this._Dados = value;
                this._PossuiRetorno = true;
            }
        }

        #endregion
    }
}
