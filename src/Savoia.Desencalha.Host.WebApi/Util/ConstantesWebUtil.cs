using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Util
{
    public static class ConstantesWebUtil
    {
        public  enum  ClienteAtualizacaoTipo { PeloCliente, PorAdmin }
        public enum UsuarioAutenticacaoTipo { Cliente, Admin, Anonimo }

        public const string USUARIO_SESSAO = "usuario-sessao";
        public const string CLIENTE_SESSAO = "cliente-sessao";


    }
}
