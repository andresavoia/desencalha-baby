using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IEmailRepository
    {

        Task EnviarAsync(
            string de,
            string deNome,
            List<string> para,
            List<string> copia,
            List<string> copiaOculta,
            List<string> anexo,
            string assunto,
            string corpo,
            string servidor,
            string usuario,
            string senha,
            int porta,
            bool SSL,
            bool UseDefaultCredentials,
            bool htmlCorpo);

    }
}
