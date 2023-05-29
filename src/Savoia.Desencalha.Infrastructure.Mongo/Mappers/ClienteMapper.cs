using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteDocument Mapper(this ClienteModel model)
        {
            if (model == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ClienteModel, ClienteDocument>(model);

            //Enderelo Cobrança
            if (model.EnderecoCobranca != null)
            {
                var cidade = new CidadeDocument();

                if (model.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = model.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = model.EnderecoCobranca.Cidade.Titulo;

                    if (model.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoDocument();
                        estado.CodEstado = model.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = model.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = model.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.EnderecoCobranca = new EnderecoDocument()
                {
                    Bairro = model.EnderecoCobranca?.Bairro,
                    Cep = model.EnderecoCobranca?.Cep,
                    Complemento = model.EnderecoCobranca?.Complemento,
                    Endereco = model.EnderecoCobranca?.Endereco,
                    Obs = model.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (model.EnderecoEntrega != null)
            {
                var cidade = new CidadeDocument();

                if (model.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = model.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = model.EnderecoEntrega.Cidade.Titulo;

                    if (model.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoDocument();
                        estado.CodEstado = model.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = model.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = model.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.EnderecoEntrega = new EnderecoDocument()
                {
                    Bairro = model.EnderecoEntrega?.Bairro,
                    Cep = model.EnderecoEntrega?.Cep,
                    Complemento = model.EnderecoEntrega?.Complemento,
                    Endereco = model.EnderecoEntrega?.Endereco,
                    Obs = model.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            if (!string.IsNullOrEmpty(model.IdCliente))
                retorno.IdCliente = MongoDB.Bson.ObjectId.Parse(model.IdCliente);

            if (!string.IsNullOrEmpty(model.IdRamoAtividade))
                retorno.IdRamoAtividade = MongoDB.Bson.ObjectId.Parse(model.IdRamoAtividade);

            return retorno;
        }

        public static ClienteModel Mapper(this ClienteDocument document)
        {
            if (document == null) return null;

            var retorno = ColecaoUtil.AtribuirValores<ClienteDocument, ClienteModel>(document);

            //Endereco Cobrança
            if (document.EnderecoCobranca != null)
            {
                var cidade = new CidadeModel();

                if (document.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = document.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = document.EnderecoCobranca.Cidade.Titulo;

                    if (document.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = document.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = document.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = document.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.EnderecoCobranca = new EnderecoModel()
                {
                    Bairro = document.EnderecoCobranca?.Bairro,
                    Cep = document.EnderecoCobranca?.Cep,
                    Complemento = document.EnderecoCobranca?.Complemento,
                    Endereco = document.EnderecoCobranca?.Endereco,
                    Obs = document.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (document.EnderecoEntrega != null)
            {
                var cidade = new CidadeModel();

                if (document.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = document.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = document.EnderecoEntrega.Cidade.Titulo;

                    if (document.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = document.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = document.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = document.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.EnderecoEntrega = new EnderecoModel()
                {
                    Bairro = document.EnderecoEntrega?.Bairro,
                    Cep = document.EnderecoEntrega?.Cep,
                    Complemento = document.EnderecoEntrega?.Complemento,
                    Endereco = document.EnderecoEntrega?.Endereco,
                    Obs = document.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }


            if (document.IdCliente != MongoDB.Bson.ObjectId.Empty)
                retorno.IdCliente = document.IdCliente.ToString();

            if (document != null && document.IdRamoAtividade != MongoDB.Bson.ObjectId.Empty)
                retorno.IdRamoAtividade = document.IdRamoAtividade.ToString();

            return retorno;
        }

    }
}
