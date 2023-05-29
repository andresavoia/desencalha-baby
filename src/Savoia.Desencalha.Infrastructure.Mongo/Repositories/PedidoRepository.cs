using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using Savoia.Desencalha.Infrastructure.Mongo.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Infrastructure.Mongo.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly MongoDbContext context = null;

        public PedidoRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<bool> ConsistirAsync(int idPedido)
        {
            List<PedidoModel> listaRetorno = new List<PedidoModel>();

            FilterDefinition<PedidoDocument> filterCompleto = Builders<PedidoDocument>.Filter.Empty;

            if (idPedido!=0)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(r => r.IdPedido != idPedido);

            
            var total = context.Pedidos.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        //public async Task LimparCarrinhoAsync(string idCliente)
        //{
        //    var filter = Builders<CarrinhoDocument>.Filter.Eq(r => r.IdCliente, MongoDB.Bson.ObjectId.Parse(idCliente));

        //    var carrinho = context.Carrinho.Find(filter).FirstOrDefault();

        //    carrinho.ValorTotal = 0;
        //    carrinho.ValorTotalComFrete = 0;
        //    carrinho.ValorTotalFrete = 0;
        //    carrinho.Itens.Clear();
            
        //    await context.Carrinho.ReplaceOneAsync(filter, carrinho); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            
        //}


        public async Task RemoverCarrinhoAsync(string idCliente)
        {
            var filter = Builders<CarrinhoDocument>.Filter.Eq(r => r.IdCliente, MongoDB.Bson.ObjectId.Parse(idCliente));
            await context.Carrinho.DeleteOneAsync(filter); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
        }
        public async Task RemoverCarrinhoPorTokenAsync(string token)
        {
            var filter = Builders<CarrinhoDocument>.Filter.Eq(r => r.Token, token);
            await context.Carrinho.DeleteOneAsync(filter); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
        }







        // Alteração PedidoVendedor
        public async Task<List<PedidoVendedorModel>> ListarPedidoVendedorAsync(string idCliente, int? idPedido = null, int? codPedidoVendedorStatus = null, int? paginaAtual = null, int? numeroRegistros = null, 
            string ordenacao = null, bool ordenacaoAsc = true, bool? isVendedor = null)
        {
            
            List<PedidoVendedorModel> listaRetorno = new List<PedidoVendedorModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<PedidoVendedorDocument> filterCompleto = Builders<PedidoVendedorDocument>.Filter.Empty;

            if(isVendedor.HasValue && isVendedor.Value && idCliente != null)
                filterCompleto = filterCompleto & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdCliente, idCliente);

            if (idPedido != null)
                filterCompleto = filterCompleto & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdPedido, idPedido);

            if (codPedidoVendedorStatus != null)
                filterCompleto = filterCompleto & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.CodPedidoVendedorStatus, codPedidoVendedorStatus);


            if (string.IsNullOrEmpty(ordenacao))
            {
                context.PedidoVendedor
                    .Find(filterCompleto)
                    .SortBy(x => x.IdPedido)
                    .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                        listaRetorno.Add(x.Mapper());
                    });
            }
            else
            {
                SortDefinition<PedidoVendedorDocument> orderBy = null;

                if (ordenacaoAsc)
                    orderBy = Builders<PedidoVendedorDocument>.Sort.Ascending(ordenacao);
                else
                    orderBy = Builders<PedidoVendedorDocument>.Sort.Descending(ordenacao);

                context.PedidoVendedor
                  .Find(filterCompleto)
                  .Sort(orderBy)
                  .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                      listaRetorno.Add(x.Mapper());
                  });
            }
            return listaRetorno;

        }

        
        // Alteração PedidoVendedor
        public async Task<List<PedidoModel>> ListarInformacaoPedidoBaseAsync(List<int> idsPedido, int? paginaAtual = null, int? numeroRegistros = null, string ordenacao = null, bool ordenacaoAsc = true, bool? isVendedor = null, string idClienteComprador = null)
        {
            List<PedidoModel> listaRetorno = new List<PedidoModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<PedidoDocument> filterCompleto = Builders<PedidoDocument>.Filter.Empty;
            
            if (idsPedido != null)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.In(r => r.IdPedido, idsPedido);

            if (isVendedor.HasValue && !isVendedor.Value)
            {
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(r => r.Cliente.IdCliente.Equals(MongoDB.Bson.ObjectId.Parse(idClienteComprador)));
            }
            

            if (string.IsNullOrEmpty(ordenacao))
            {
                context.Pedidos
                    .Find(filterCompleto)
                    .SortBy(x => x.IdPedido)
                    .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                        listaRetorno.Add(x.Mapper());
                    });
            }
            else
            {
                SortDefinition<PedidoDocument> orderBy = null;

                if (ordenacaoAsc)
                    orderBy = Builders<PedidoDocument>.Sort.Ascending(ordenacao);
                else
                    orderBy = Builders<PedidoDocument>.Sort.Descending(ordenacao);

                context.Pedidos
                  .Find(filterCompleto)
                  .Sort(orderBy)
                  .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                      listaRetorno.Add(x.Mapper());
                  });
            }
            return listaRetorno;
        }










        public async Task<List<PedidoModel>> ListarAsync(int? idPedido = null, int ? codPedidoStatus = null, string cliente = null, DateTime? dataCadastroInicial = null, DateTime? dataCadastroFinal = null, string idProduto = null, 
            string idCliente = null, string cnpjCliente = null, int? paginaAtual = null, int? numeroRegistros = null, string ordenacao = null, bool ordenacaoAsc = true)
        {
            List<PedidoModel> listaRetorno = new List<PedidoModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<PedidoDocument> filterCompleto = Builders<PedidoDocument>.Filter.Empty;
            
            if (codPedidoStatus !=null)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Eq(r => r.CodPedidoStatus, codPedidoStatus);

            if (idPedido != null)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Eq(r => r.IdPedido, idPedido);

            if (!string.IsNullOrEmpty(idCliente))
            {
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(x => x.Cliente.IdCliente == MongoDB.Bson.ObjectId.Parse(idCliente));
            }
            else
                if (!string.IsNullOrEmpty(cliente))
                    filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(x => x.Cliente.NomeOuRazao.ToLower().Contains(cliente));

            if (!string.IsNullOrEmpty(cnpjCliente))
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(x => x.Cliente.CpfOuCnpj == cnpjCliente);

            if (dataCadastroInicial != null)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(s => s.DataCadastro >= dataCadastroInicial &&
                                                                                        s.DataCadastro <= dataCadastroFinal);
            //if (idProduto != null)
            //    filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Where(r => r.Itens.Any(fb => fb.IdProduto == idProduto));

            var total = context.Pedidos.Find(filterCompleto).CountDocuments();

            if (string.IsNullOrEmpty(ordenacao)){
                context.Pedidos
                    .Find(filterCompleto)
                    .SortBy(x => x.IdPedido)
                    .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                        listaRetorno.Add(x.Mapper());
                    });
            }
            else
            {
                SortDefinition<PedidoDocument> orderBy = null;

                if (ordenacaoAsc)
                    orderBy = Builders<PedidoDocument>.Sort.Ascending(ordenacao);
                else
                    orderBy = Builders<PedidoDocument>.Sort.Descending(ordenacao);

                context.Pedidos
                  .Find(filterCompleto)
                  .Sort(orderBy)
                  .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                      listaRetorno.Add(x.Mapper());
                  });
            }
            return listaRetorno;
        }

        public async Task<PedidoModel> ManterAsync(PedidoModel model, bool? criarPedido)
        {
            var document = model.Mapper();

            if (criarPedido == true)
            {
                await context.Pedidos.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<PedidoDocument>.Filter.Eq("_id", document.IdPedido);
                await context.Pedidos.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.Pedidos.Indexes.CreateOneAsync(
                Builders<PedidoDocument>.IndexKeys.Ascending(x => x.IdPedido)
                );

            context.Pedidos.Indexes.CreateOneAsync(
                Builders<PedidoDocument>.IndexKeys.Ascending(x => x.CodPedidoStatus)
            );

            context.Pedidos.Indexes.CreateOneAsync(
                Builders<PedidoDocument>.IndexKeys.Ascending(x => x.Cliente.IdCliente)
            );

            
            return document.Mapper();
        }


        public async Task<PedidoVendedorModel> ManterPedidoVendedorAsync(PedidoVendedorModel model, bool? criarPedido)
        {
            var document = model.Mapper();


            if (criarPedido == true)
            {
                await context.PedidoVendedor.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdPedido, document.IdPedido) 
                    & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdCliente, document.IdCliente);

                await context.PedidoVendedor.ReplaceOneAsync(filter, document); 
            }

            //Criando indices
            context.PedidoVendedor.Indexes.CreateOneAsync(
                Builders<PedidoVendedorDocument>.IndexKeys.Ascending(x => x.IdPedido)
            );

            context.PedidoVendedor.Indexes.CreateOneAsync(
                Builders<PedidoVendedorDocument>.IndexKeys.Ascending(x => x.IdCliente)
            );

            var indexDefinition = Builders<PedidoVendedorDocument>.IndexKeys.Combine(
                Builders<PedidoVendedorDocument>.IndexKeys.Ascending(f => f.Itens),
                Builders<PedidoVendedorDocument>.IndexKeys.Ascending(f => f.Itens[-1].IdProduto));

            context.PedidoVendedor.Indexes.CreateOneAsync(indexDefinition);

            return document.Mapper();
        }



        // Alteração PedidoVendedor
        public async Task<List<PedidoVendedorModel>> ObterPedidoVendedorAsync(string idCliente, int idPedido, bool? isVendedor = null)
        {
            FilterDefinition<PedidoVendedorDocument> filterCompleto = Builders<PedidoVendedorDocument>.Filter.Empty;

            if(isVendedor.HasValue && isVendedor.Value)
                filterCompleto = filterCompleto & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdCliente, idCliente);

            if (idPedido != 0)
                filterCompleto = filterCompleto & Builders<PedidoVendedorDocument>.Filter.Eq(r => r.IdPedido, idPedido);

            return context.PedidoVendedor.Find(filterCompleto).ToList().Mapper();
        }



        public async Task<PedidoModel> ObterAsync(int idPedido)
        {
            List<PedidoModel> listaRetorno = new List<PedidoModel>();

            FilterDefinition<PedidoDocument> filterCompleto = Builders<PedidoDocument>.Filter.Empty;

            if (idPedido!=0)
                filterCompleto = filterCompleto & Builders<PedidoDocument>.Filter.Eq(r => r.IdPedido, idPedido);

            return context.Pedidos.Find(filterCompleto).FirstOrDefault().Mapper();
             
        }

        public async Task<int?> ObterPedidoUltimoAsync()
        {
            var idPedido =  context.Pedidos.Find(Builders<PedidoDocument>.Filter.Empty).SortByDescending(x => x.IdPedido)?.FirstOrDefaultAsync()?.Result?.IdPedido;

            return idPedido;
        }

        public async Task<long> ObterTotalStatusAsync(int codPedidoStatus)
        {
            return context.Pedidos.Find(Builders<PedidoDocument>.Filter.Where(r => r.CodPedidoStatus.Equals((int)codPedidoStatus))).CountDocumentsAsync().Result;
        }

        public async Task<bool> ManterCarrinhoAsync(CarrinhoModel model, bool? carrinhoPorToken = null)
        {
            try
            {
                var document = model.Mapper();


                FilterDefinition<CarrinhoDocument> filter;
                if (carrinhoPorToken==true)
                    filter = Builders<CarrinhoDocument>.Filter.Eq(r => r.Token, model.Token);
                else
                    filter = Builders<CarrinhoDocument>.Filter.Eq(r => r.IdCliente, MongoDB.Bson.ObjectId.Parse(model.IdCliente));

                var registro = await context.Carrinho.Find(filter).FirstOrDefaultAsync();

                if(registro==null)
                    await context.Carrinho.InsertOneAsync(document);
                else
                    await context.Carrinho.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 


                //Criando indices
                context.Carrinho.Indexes.CreateOneAsync(
                    Builders<CarrinhoDocument>.IndexKeys.Ascending(x => x.IdCliente)
                    );

                //Criando indices
                context.Carrinho.Indexes.CreateOneAsync(
                    Builders<CarrinhoDocument>.IndexKeys.Ascending(x => x.Token)
                    );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<CarrinhoModel> ObterCarrinhoAsync(string idCliente)
        {
            List<CarrinhoModel> listaRetorno = new List<CarrinhoModel>();

            FilterDefinition<CarrinhoDocument> filterCompleto = Builders<CarrinhoDocument>.Filter.Empty;

            filterCompleto = filterCompleto & Builders<CarrinhoDocument>.Filter.Eq(r => r.IdCliente, MongoDB.Bson.ObjectId.Parse(idCliente));

            return context.Carrinho.Find(filterCompleto).FirstOrDefault().Mapper();
        }

        public async Task<CarrinhoModel> ObterCarrinhoPorTokenAsync(string token)
        {
            List<CarrinhoModel> listaRetorno = new List<CarrinhoModel>();

            FilterDefinition<CarrinhoDocument> filterCompleto = Builders<CarrinhoDocument>.Filter.Empty;

            filterCompleto = filterCompleto & Builders<CarrinhoDocument>.Filter.Eq(r => r.Token, token);

            return context.Carrinho.Find(filterCompleto).FirstOrDefault().Mapper();
        }

        public async Task<bool> AlterarValidadeCarrinhoAsync(string token)
        {
            FilterDefinition<CarrinhoDocument> filterCompleto = Builders<CarrinhoDocument>.Filter.Empty;
            filterCompleto = filterCompleto & Builders<CarrinhoDocument>.Filter.Eq(r => r.Token, token);
            var resultado = context.Carrinho.Find(filterCompleto)?.FirstOrDefault();

            if (resultado != null)
            {
                resultado.DataExpiracaoToken = DateTime.Now.AddMinutes(ConstantesUtil.TIMEOUT_SESSAO_MINUTOS);
                await context.Carrinho.ReplaceOneAsync(filterCompleto, resultado);
                return true;
            }
            else
                return false;
        }
    }
}
