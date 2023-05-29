using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using Savoia.Desencalha.Host.WebApi.Messages.Pedido;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using Savoia.Desencalha.Infrastructure.Mongo.Mappers;
using System.Collections.Generic;

namespace Savoia.Desencalha.Host.WebApi.Mappers
{
    public static class PedidoMapper
    {

        public static List<PedidoDto> MapperPedidoBaseComprador(this List<PedidoModel> listaPedidoBase)
        {
            if (listaPedidoBase == null) return null;
            List<PedidoDto> retorno = new List<PedidoDto>();

            listaPedidoBase.ForEach(x => {
                retorno.Add(x.MapperToPedidoDto());
            });

            return retorno;
        }

        public static PedidoVendedorDto MapperToPedidoVendedorDto(this PedidoVendedorModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoVendedorModel, PedidoVendedorDto>(model);
            retorno.Logs = ColecaoUtil.AtribuirValoresLista<PedidoLogModel, PedidoLogDto>(model.Logs);
            retorno.Anexos = ColecaoUtil.AtribuirValoresLista<PedidoAnexoModel, PedidoAnexoDto>(model.Anexos);
            retorno.Rastreio = ColecaoUtil.AtribuirValores<PedidoRastreioModel, PedidoRastreioDto>(model.Rastreio);

            retorno.Itens = model.Itens.MapperToPedidoItemDto();

            return retorno;
        }



        public static List<PedidoDto> Mapper(this List<PedidoVendedorModel> listaPedidoVendedor, List<PedidoModel> listaPedidoBase)
        {
            if (listaPedidoVendedor == null) return null;
            List<PedidoDto> retorno = new List<PedidoDto>();

            listaPedidoVendedor.ForEach(x => {

                var pedidoBase = listaPedidoBase.Where(pb => pb.IdPedido == x.IdPedido).FirstOrDefault();

                retorno.Add(x.Mapper(pedidoBase));
            });

            return retorno;
        }

        public static List<PedidoVendedorDto> Mapper(this List<PedidoVendedorModel> listaPedidoVendedor)
        {
            if (listaPedidoVendedor == null) return null;
            List<PedidoVendedorDto> retorno = new List<PedidoVendedorDto>();

            listaPedidoVendedor.ForEach(x => {

                var pedidoVendedorDto = new PedidoVendedorDto();
                pedidoVendedorDto.IdVendedor = x.IdCliente;
                pedidoVendedorDto.ValorTotal = x.ValorTotal;
                pedidoVendedorDto.ValorTotalFrete = x.ValorTotalFrete;
                pedidoVendedorDto.ValorTotalComFrete = x.ValorTotalComFrete;
                pedidoVendedorDto.DataPrevisaoEntrega = x.DataPrevisaoEntrega;
                pedidoVendedorDto.DescFreteTipo = ConstantesUtil.ObterFreteTipo(x.CodFreteTipoUtilizado);
                pedidoVendedorDto.CodPedidoVendedorStatus = x.CodPedidoVendedorStatus;

                pedidoVendedorDto.Logs = ColecaoUtil.AtribuirValoresLista<PedidoLogModel, PedidoLogDto>(x.Logs);
                pedidoVendedorDto.Anexos = ColecaoUtil.AtribuirValoresLista<PedidoAnexoModel, PedidoAnexoDto>(x.Anexos);
                pedidoVendedorDto.Rastreio = ColecaoUtil.AtribuirValores<PedidoRastreioModel, PedidoRastreioDto>(x.Rastreio);

                pedidoVendedorDto.Itens = x.Itens.MapperToPedidoItemDto();

                retorno.Add(pedidoVendedorDto);
            });

            return retorno;
        }



        public static PedidoDto Mapper(this PedidoVendedorModel pedidoVendedor, PedidoModel pedidoBase)
        {
            if (pedidoVendedor == null) return null;

            var retorno = new PedidoDto();

            retorno.IdPedido = pedidoBase.IdPedido;
            retorno.CodPedidoStatus = pedidoBase.CodPedidoStatus;
            retorno.ValorTotal = pedidoVendedor.ValorTotal;
            retorno.ValorTotalFrete = pedidoVendedor.ValorTotalFrete;
            retorno.ValorTotalComFrete = pedidoVendedor.ValorTotalComFrete;
            retorno.DataCadastro = pedidoBase.DataCadastro;
            retorno.DataAlteracao = pedidoBase.DataAlteracao;
            retorno.UsuarioAlteracao = pedidoVendedor.UsuarioAlteracao;

            retorno.PedidoVendedor = new List<PedidoVendedorDto>();
            retorno.PedidoVendedor.Add(new PedidoVendedorDto { CodPedidoVendedorStatus = pedidoVendedor.CodPedidoVendedorStatus });

            retorno.Itens = pedidoVendedor.Itens.MapperToPedidoItemDto();

            retorno.Cliente = ColecaoUtil.AtribuirValores<ClienteModel, ClienteDto>(pedidoBase.Cliente);


            if (!string.IsNullOrEmpty(pedidoBase?.Cliente?.IdCliente))
                retorno.Cliente.IdCliente = pedidoBase.Cliente.IdCliente.ToString();

            if (pedidoBase.Cliente.EnderecoCobranca != null)
            {
                var cidade = new CidadeDto();

                if (pedidoBase.Cliente.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = pedidoBase.Cliente.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = pedidoBase.Cliente.EnderecoCobranca.Cidade.Titulo;

                    if (pedidoBase.Cliente.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = pedidoBase.Cliente.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = pedidoBase.Cliente.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = pedidoBase.Cliente.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoCobranca = new Dtos.EnderecoDto()
                {
                    Bairro = pedidoBase.Cliente.EnderecoCobranca?.Bairro,
                    Cep = pedidoBase.Cliente.EnderecoCobranca?.Cep,
                    Complemento = pedidoBase.Cliente.EnderecoCobranca?.Complemento,
                    Endereco = pedidoBase.Cliente.EnderecoCobranca?.Endereco,
                    Obs = pedidoBase.Cliente.EnderecoCobranca?.Obs,
                    Cidade = cidade
                };
            }

            //Endereço Entrega
            if (pedidoBase.Cliente.EnderecoEntrega != null)
            {
                var cidade = new CidadeDto();

                if (pedidoBase.Cliente.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = pedidoBase.Cliente.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = pedidoBase.Cliente.EnderecoEntrega.Cidade.Titulo;

                    if (pedidoBase.Cliente.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = pedidoBase.Cliente.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = pedidoBase.Cliente.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = pedidoBase.Cliente.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoEntrega = new Dtos.EnderecoDto()
                {
                    Bairro = pedidoBase.Cliente.EnderecoEntrega?.Bairro,
                    Cep = pedidoBase.Cliente.EnderecoEntrega?.Cep,
                    Complemento = pedidoBase.Cliente.EnderecoEntrega?.Complemento,
                    Endereco = pedidoBase.Cliente.EnderecoEntrega?.Endereco,
                    Obs = pedidoBase.Cliente.EnderecoEntrega?.Obs,
                    Cidade = cidade
                };
            }

            return retorno;
        }






        public static PedidoDto MapperToPedidoDto(this PedidoModel model)
        {
            if (model == null) return null;
            var retorno = ColecaoUtil.AtribuirValores<PedidoModel, PedidoDto>(model);

            retorno.Cliente = ColecaoUtil.AtribuirValores<ClienteModel, ClienteDto>(model.Cliente);

            if (!string.IsNullOrEmpty(model?.Cliente?.IdCliente))
             retorno.Cliente.IdCliente = model.Cliente.IdCliente.ToString();

            if (model.Cliente.EnderecoCobranca != null)
            {
                var cidade = new CidadeDto();

                if (model.Cliente.EnderecoCobranca.Cidade != null)
                {

                    cidade.CodCidade = model.Cliente.EnderecoCobranca.Cidade.CodCidade;
                    cidade.Titulo = model.Cliente.EnderecoCobranca.Cidade.Titulo;

                    if (model.Cliente.EnderecoCobranca.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = model.Cliente.EnderecoCobranca.Cidade.Estado.CodEstado;
                        estado.Titulo = model.Cliente.EnderecoCobranca.Cidade.Estado.Titulo;
                        estado.UF = model.Cliente.EnderecoCobranca.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoCobranca = new Dtos.EnderecoDto()
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
                var cidade = new CidadeDto();

                if (model.Cliente.EnderecoEntrega.Cidade != null)
                {

                    cidade.CodCidade = model.Cliente.EnderecoEntrega.Cidade.CodCidade;
                    cidade.Titulo = model.Cliente.EnderecoEntrega.Cidade.Titulo;

                    if (model.Cliente.EnderecoEntrega.Cidade.Estado != null)
                    {
                        var estado = new EstadoDto();
                        estado.CodEstado = model.Cliente.EnderecoEntrega.Cidade.Estado.CodEstado;
                        estado.Titulo = model.Cliente.EnderecoEntrega.Cidade.Estado.Titulo;
                        estado.UF = model.Cliente.EnderecoEntrega.Cidade.Estado.UF;
                        cidade.Estado = estado;
                    }
                }

                retorno.Cliente.EnderecoEntrega = new Dtos.EnderecoDto()
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

        public static List<PedidoDto> Mapper(this List<PedidoModel> lista)
        {
            if (lista == null) return null;
            List<PedidoDto> retorno = new List<PedidoDto>();

            lista.ForEach(x => {
                retorno.Add(x.MapperToPedidoDto());
            });

            return retorno;
        }

        public static List<PedidoItemDto> MapperToPedidoItemDto(this List<PedidoItemModel> lista)
        {
            return ColecaoUtil.AtribuirValoresLista<PedidoItemModel, PedidoItemDto>(lista);
        }

        public static List<PedidoItemModel> Mapper(this List<PedidoItemDto> lista)
        {
            return ColecaoUtil.AtribuirValoresLista<PedidoItemDto,PedidoItemModel>(lista);
        }
        
        public static CarrinhoDto MapperToDto(this CarrinhoModel item)
        {
            var resultado = ColecaoUtil.AtribuirValores<CarrinhoModel, CarrinhoDto>(item);
            resultado.Itens = item.Itens.MapperToPedidoItemDto();

            return resultado;
        }

        public static CarrinhoModel Mapper(this CarrinhoDto item)
        {
            var resultado = ColecaoUtil.AtribuirValores<CarrinhoDto, CarrinhoModel>(item);
            resultado.Itens = item.Itens.Mapper();

            return resultado;
        }

    }
}
