using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using System.Collections.Generic;

namespace Savoia.Desencalha.Infrastructure.Mongo.Mappers
{
    public static class PedidoMapper
    {
        public static PedidoDocument Mapper(this PedidoModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoModel, PedidoDocument>(model);

            retorno.Cliente = ColecaoUtil.AtribuirValores<ClienteModel, ClienteDocument>(model.Cliente);

            if (!string.IsNullOrEmpty(model.Cliente.IdCliente))
                retorno.Cliente.IdCliente = MongoDB.Bson.ObjectId.Parse(model.Cliente.IdCliente);

            if (model.Cliente.EnderecoCobranca != null)
            {
                var cidade = new CidadeDocument();

                if (model.Cliente.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = model.Cliente.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = model.Cliente.EnderecoCobranca.Cidade.Titulo;

                    if (model.Cliente.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoDocument();
                        estado.CodEstado = model.Cliente.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = model.Cliente.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = model.Cliente.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoCobranca = new EnderecoDocument()
                {
                    Bairro = model.Cliente.EnderecoCobranca?.Bairro,
                    Cep = model.Cliente.EnderecoCobranca?.Cep,
                    Complemento = model.Cliente.EnderecoCobranca?.Complemento,
                    Endereco = model.Cliente.EnderecoCobranca?.Endereco,
                    Obs = model.Cliente.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (model.Cliente.EnderecoEntrega != null)
            {
                var cidade = new CidadeDocument();

                if (model.Cliente.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = model.Cliente.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = model.Cliente.EnderecoEntrega.Cidade.Titulo;

                    if (model.Cliente.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoDocument();
                        estado.CodEstado = model.Cliente.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = model.Cliente.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = model.Cliente.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoEntrega = new EnderecoDocument()
                {
                    Bairro = model.Cliente.EnderecoEntrega?.Bairro,
                    Cep = model.Cliente.EnderecoEntrega?.Cep,
                    Complemento = model.Cliente.EnderecoEntrega?.Complemento,
                    Endereco = model.Cliente.EnderecoEntrega?.Endereco,
                    Obs = model.Cliente.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            return retorno;
        }

        public static PedidoModel Mapper(this PedidoDocument document)
        {
            if (document == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoDocument,PedidoModel >(document);

            retorno.Cliente = ColecaoUtil.AtribuirValores<ClienteDocument, ClienteModel>(document.Cliente);
            retorno.Cliente.IdCliente = document.Cliente.IdCliente.ToString();
            
            if (document.Cliente.EnderecoCobranca != null)
            {
                var cidade = new CidadeModel();

                if (document.Cliente.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = document.Cliente.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = document.Cliente.EnderecoCobranca.Cidade.Titulo;

                    if (document.Cliente.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = document.Cliente.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = document.Cliente.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = document.Cliente.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoCobranca = new EnderecoModel()
                {
                    Bairro = document.Cliente.EnderecoCobranca?.Bairro,
                    Cep = document.Cliente.EnderecoCobranca?.Cep,
                    Complemento = document.Cliente.EnderecoCobranca?.Complemento,
                    Endereco = document.Cliente.EnderecoCobranca?.Endereco,
                    Obs = document.Cliente.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (document.Cliente.EnderecoEntrega != null)
            {
                var cidade = new CidadeModel();

                if (document.Cliente.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = document.Cliente.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = document.Cliente.EnderecoEntrega.Cidade.Titulo;

                    if (document.Cliente.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoModel();
                        estado.CodEstado = document.Cliente.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = document.Cliente.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = document.Cliente.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoEntrega = new EnderecoModel()
                {
                    Bairro = document.Cliente.EnderecoEntrega?.Bairro,
                    Cep = document.Cliente.EnderecoEntrega?.Cep,
                    Complemento = document.Cliente.EnderecoEntrega?.Complemento,
                    Endereco = document.Cliente.EnderecoEntrega?.Endereco,
                    Obs = document.Cliente.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }
            
            return retorno;
        }


        public static PedidoVendedorDocument Mapper(this PedidoVendedorModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoVendedorModel, PedidoVendedorDocument>(model);
            retorno.Logs = ColecaoUtil.AtribuirValoresLista<PedidoLogModel,PedidoLogDocument>(model.Logs);
            retorno.Anexos = ColecaoUtil.AtribuirValoresLista<PedidoAnexoModel, PedidoAnexoDocument>(model.Anexos);
            retorno.Rastreio = ColecaoUtil.AtribuirValores<PedidoRastreioModel, PedidoRastreioDocument>(model.Rastreio);

            retorno.Itens = model.Itens.Mapper();

            return retorno;
        }



        public static List<PedidoVendedorModel> Mapper(this List<PedidoVendedorDocument> lista)
        {
            if (lista == null) return null;

            var retorno = new List<PedidoVendedorModel>();

            foreach (var document in lista)
            {
                retorno.Add(document.Mapper());
            }


            return retorno;
        }

        public static PedidoVendedorModel Mapper(this PedidoVendedorDocument document)
        {
            if (document == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoVendedorDocument, PedidoVendedorModel>(document);
            retorno.Logs = ColecaoUtil.AtribuirValoresLista<PedidoLogDocument, PedidoLogModel>(document.Logs);
            retorno.Anexos = ColecaoUtil.AtribuirValoresLista<PedidoAnexoDocument, PedidoAnexoModel> (document.Anexos);
            retorno.Rastreio = ColecaoUtil.AtribuirValores<PedidoRastreioDocument, PedidoRastreioModel>(document.Rastreio);

            retorno.Itens = document.Itens.Mapper();

            return retorno;
        }




        public static List<PedidoItemDocument> Mapper(this List<PedidoItemModel> lista)
        {
            if (lista == null) return null;

            return ColecaoUtil.AtribuirValoresLista<PedidoItemModel, PedidoItemDocument>(lista);
        }

        public static List<PedidoItemModel> Mapper(this List<PedidoItemDocument> lista)
        {
            if (lista == null) return null;

            return ColecaoUtil.AtribuirValoresLista<PedidoItemDocument, PedidoItemModel>(lista);
        }

        public static CarrinhoDocument Mapper(this CarrinhoModel item)
        {
            if (item == null) return null;

            var resultado = ColecaoUtil.AtribuirValores<CarrinhoModel, CarrinhoDocument>(item);

            if (!string.IsNullOrEmpty(item.IdCliente))
                resultado.IdCliente = MongoDB.Bson.ObjectId.Parse(item.IdCliente);

            resultado.Itens = item.Itens.Mapper();

            return resultado;
        }

        public static CarrinhoModel Mapper(this CarrinhoDocument item)
        {
            if (item == null) return null;

            var resultado = ColecaoUtil.AtribuirValores<CarrinhoDocument, CarrinhoModel>(item);
            resultado.IdCliente = item.IdCliente.ToString();
            resultado.Itens = item.Itens?.Mapper();

            return resultado;
        }

    }
}
