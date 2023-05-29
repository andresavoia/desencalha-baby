using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using Savoia.Desencalha.Host.WebApi.Messages.Cliente;
using Savoia.Desencalha.Host.WebApi.Util;
using System.Collections.Generic;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteModel Mapper(this ManterClienteRequest request, ClienteModel modelOriginal, ConstantesWebUtil.ClienteAtualizacaoTipo tipoAtualizacao)
        {
            if (request == null) return null;

            ClienteModel model = ColecaoUtil.AtribuirValores<ManterClienteRequest, ClienteModel>(request);
            
            //EnderecoCobrança
            if (request.EnderecoCobranca != null)
            {
                var cidade = new CidadeModel();

                if (request.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = request.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = request.EnderecoCobranca.Cidade.Titulo;

                    if (request.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = request.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = request.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = request.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                model.EnderecoCobranca = new EnderecoModel()
                {
                    Bairro = request.EnderecoCobranca?.Bairro,
                    Cep = request.EnderecoCobranca?.Cep,
                    Complemento = request.EnderecoCobranca?.Complemento,
                    Endereco = request.EnderecoCobranca?.Endereco,
                    Obs = request.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //EnderecoEntrega
            if (request.EnderecoEntrega != null)
            {
                var cidade = new CidadeModel();

                if (request.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = request.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = request.EnderecoEntrega.Cidade.Titulo;

                    if (request.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = request.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = request.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = request.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                model.EnderecoEntrega = new EnderecoModel()
                {
                    Bairro = request.EnderecoEntrega?.Bairro,
                    Cep = request.EnderecoEntrega?.Cep,
                    Complemento = request.EnderecoEntrega?.Complemento,
                    Endereco = request.EnderecoEntrega?.Endereco,
                    Obs = request.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            //Pegar do modelOriginal
            model.DataCadastroSite = modelOriginal.DataCadastroSite;
            model.UsuarioCadastro = modelOriginal.UsuarioCadastro;
            model.CampoAbertoSistema = modelOriginal.CampoAbertoSistema;
            model.Senha = modelOriginal.Senha;
            model.Salt = modelOriginal.Salt;

            if (tipoAtualizacao.Equals(ConstantesWebUtil.ClienteAtualizacaoTipo.PeloCliente))
            {
                model.CodInterno = modelOriginal.CodInterno;
                model.CodClienteStatus = modelOriginal.CodClienteStatus;
                model.CodFreteTipo = modelOriginal.CodFreteTipo;
                model.UsuarioAutorizacao = modelOriginal.UsuarioAutorizacao;
                model.ValorFreteFixo = modelOriginal.ValorFreteFixo;
                model.DiasPrazoEntregaFixo = modelOriginal.DiasPrazoEntregaFixo;
                model.CpfOuCnpj = modelOriginal.CpfOuCnpj;
                model.RgOuInscricao = modelOriginal.RgOuInscricao;

            }

            model.Categorias = request.Categorias;
            
            return model;
        }

        public static ClienteModel Mapper(this CriarClienteRequest request)
        {
            if (request == null) return null;

            ClienteModel model = ColecaoUtil.AtribuirValores<CriarClienteRequest, ClienteModel>(request);

            //Endereço Cobrança
            if (request.EnderecoCobranca != null)
            {
                var cidade = new CidadeModel();

                if (request.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = request.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = request.EnderecoCobranca.Cidade.Titulo;

                    if (request.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = request.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = request.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = request.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                model.EnderecoCobranca = new EnderecoModel()
                {
                    Bairro = request.EnderecoCobranca?.Bairro,
                    Cep = request.EnderecoCobranca?.Cep,
                    Complemento = request.EnderecoCobranca?.Complemento,
                    Endereco = request.EnderecoCobranca?.Endereco,
                    Obs = request.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (request.EnderecoEntrega != null)
            {
                var cidade = new CidadeModel();

                if (request.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = request.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = request.EnderecoEntrega.Cidade.Titulo;

                    if (request.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = request.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = request.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = request.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                model.EnderecoEntrega = new EnderecoModel()
                {
                    Bairro = request.EnderecoEntrega?.Bairro,
                    Cep = request.EnderecoEntrega?.Cep,
                    Complemento = request.EnderecoEntrega?.Complemento,
                    Endereco = request.EnderecoEntrega?.Endereco,
                    Obs = request.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            return model;
        }

        public static ClienteDto Mapper(this ClienteModel model)
        {

            if (model == null) return null;

            ClienteDto dto = ColecaoUtil.AtribuirValores<ClienteModel, ClienteDto>(model);

            if (model.EnderecoCobranca != null)
            {
                var cidade = new CidadeDto();

                if (model.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = model.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = model.EnderecoCobranca.Cidade.Titulo;

                    if (model.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = model.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = model.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = model.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                dto.EnderecoCobranca = new EnderecoDto()
                {
                    Bairro = model.EnderecoCobranca?.Bairro,
                    Cep = model.EnderecoCobranca?.Cep,
                    Complemento = model.EnderecoCobranca?.Complemento,
                    Endereco = model.EnderecoCobranca?.Endereco,
                    Obs = model.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            if (model.EnderecoEntrega != null)
            {
                var cidade = new CidadeDto();

                if (model.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = model.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = model.EnderecoEntrega.Cidade.Titulo;

                    if (model.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = model.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = model.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = model.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                dto.EnderecoEntrega = new EnderecoDto()
                {
                    Bairro = model.EnderecoEntrega?.Bairro,
                    Cep = model.EnderecoEntrega?.Cep,
                    Complemento = model.EnderecoEntrega?.Complemento,
                    Endereco = model.EnderecoEntrega?.Endereco,
                    Obs = model.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            return dto;
        }

        public static List<ClienteDto> Mapper(this List<ClienteModel> lista)
        {
            if (lista == null) return null;

            var retorno = new List<ClienteDto>();
            lista.ForEach(x =>
            {
                retorno.Add(x.Mapper());
            });
            return retorno;
        }

    }
}
