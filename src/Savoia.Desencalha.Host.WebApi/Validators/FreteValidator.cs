using Savoia.Desencalha.Host.WebApi.Messages.Frete;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class FreteValidator : IFreteValidator
    {
        
        public ListarFreteEstadoResponse Validar(string idCliente)
        {

            if (string.IsNullOrEmpty(idCliente))
                return GetResponseErrorValidation<ListarFreteEstadoResponse>("","Selecione um cliente");

            return new ListarFreteEstadoResponse();
        }
    }
}
