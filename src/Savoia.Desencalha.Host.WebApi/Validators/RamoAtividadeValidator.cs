using Savoia.Desencalha.Host.WebApi.Messages.RamoAtividade;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class RamoAtividadeValidator : IRamoAtividadeValidator
    {
        internal IRamoAtividadeRepository usuarioRepository;
        internal IClienteRepository clienteRepository;
        
        public RamoAtividadeValidator(IRamoAtividadeRepository usuarioRepository,
                                    IClienteRepository clienteRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clienteRepository = clienteRepository;
        }

        public ManterRamoAtividadeResponse Validar(ManterRamoAtividadeRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterRamoAtividadeResponse>();

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                return GetResponseErrorValidation<ManterRamoAtividadeResponse>(nameof(request.Titulo),"Preencha o campo titulo");
            }

            if (usuarioRepository.ConsistirAsync(request.IdRamoAtividade,request.Titulo).Result)
                return GetResponseErrorValidation<ManterRamoAtividadeResponse>("",MessageResource.COMUM_REGISTRO_EXISTENTE);

            return new ManterRamoAtividadeResponse();
        }

        public ListarRamoAtividadeResponse Validar( string titulo, bool? ativo)
        {
            //if (string.IsNullOrWhiteSpace(codInterno) && string.IsNullOrWhiteSpace(titulo) && ativo ==null)
            //    return GetResponseError<ListarRamoAtividadeResponse>(MessageResource.COMUM_PESQUISA_PREENCHA_CAMPO);

            return new ListarRamoAtividadeResponse();
        }

        public ExcluirRamoAtividadeResponse Validar(string idRamoAtividade)
        {
            var existe = clienteRepository.ConsistirAsync(idRamoAtividade).Result;

            if (existe)
                return GetResponseErrorValidation<ExcluirRamoAtividadeResponse>("",MessageResource.COMUM_REGISTRO_ASSOCIADO);

            return new ExcluirRamoAtividadeResponse();
        }
    }
}
