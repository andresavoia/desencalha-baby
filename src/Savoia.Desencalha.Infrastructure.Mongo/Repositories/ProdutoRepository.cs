using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly MongoDbContext context = null;

        public ProdutoRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<bool> ConsistirAsync(string idProduto, string idCategoria, string codInterno, string titulo, string idCliente)
        {
            List<ProdutoModel> listaRetorno = new List<ProdutoModel>();

            FilterDefinition<ProdutoDocument> filterCompleto = Builders<ProdutoDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(idProduto))
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdProduto != MongoDB.Bson.ObjectId.Parse(idProduto));

            filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdCliente == MongoDB.Bson.ObjectId.Parse(idCliente));
            filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdCategoria == MongoDB.Bson.ObjectId.Parse(idCategoria));

            filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Or(
                Builders<ProdutoDocument>.Filter.Eq(r => r.CodInterno, codInterno),
                Builders<ProdutoDocument>.Filter.Where(r => r.Titulo.Equals(titulo))
                );

            var total = context.Produtos.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        public async Task<long> ExcluirAsync(string id)
        {
            var filter = Builders<ProdutoDocument>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));

            var result = context.Produtos.DeleteOne(filter);
            return result.DeletedCount;
        }

        public async Task<List<ProdutoModel>> ListarAsync(string codInterno, string titulo, string idCategoria = null,
                                                          bool? ativo = null, bool? promocao = null, bool? lancamento = null,
                                                          string ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null, 
                                                          string? IdCliente = null, bool? webAppAdmin = null, List<string> categorias = null)
        {
            List<ProdutoModel> listaRetorno = new List<ProdutoModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<ProdutoDocument> filterCompleto = Builders<ProdutoDocument>.Filter.Empty;
            
            if (!string.IsNullOrEmpty(titulo))
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(x => x.Titulo.ToLower().Contains(titulo));

            if (!string.IsNullOrEmpty(codInterno))
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Eq(r => r.CodInterno, codInterno);

            if (idCategoria != null)
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdCategoria.Equals(MongoDB.Bson.ObjectId.Parse(idCategoria)));

            if (ativo != null)
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.Ativo == ativo);

            if (promocao != null)
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.ProdutoPromocao == promocao);

            if (lancamento != null)
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.ProdutoLancamento == lancamento);




            //Se for webAppAdmin (vendedor), lista apenas produtos do vendedor e ignora categorias preferenciais
            //senão, se for cliente/vendedor/comprador e estiver logado no site, não lista seus produtos e considera categorias preferenciais quando tiver
            if (webAppAdmin.HasValue && webAppAdmin.Value)
            {
                if (IdCliente != null)
                    filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdCliente.Equals(MongoDB.Bson.ObjectId.Parse(IdCliente)));
            }
            else
            {
                if (IdCliente != null)
                    filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => !r.IdCliente.Equals(MongoDB.Bson.ObjectId.Parse(IdCliente)));


                if (categorias != null)
                {
                    List<MongoDB.Bson.ObjectId> ids = new List<MongoDB.Bson.ObjectId>();

                    foreach (var item in categorias)
                    {
                        ids.Add(MongoDB.Bson.ObjectId.Parse(item));
                    }

                    filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.In("IdCategoria", ids.ToArray());
                }
            }


            var total = context.Produtos.Find(filterCompleto). CountDocuments();

            SortDefinition<ProdutoDocument> orderBy = null;

            if (string.IsNullOrEmpty(ordenacao) || ordenacao.ToLower() == "titulo")
            {
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");
            }
            else if(ordenacao.ToLower() == "menorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("ValorVenda");
            else if (ordenacao.ToLower() == "maiorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Descending("ValorVenda");
            else
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");

            context.Produtos.Find(filterCompleto)
                .Sort(orderBy)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                listaRetorno.Add(x.Mapper());
            });

            if (listaRetorno != null && listaRetorno.Count > 0)
                listaRetorno[0].TotalRegistros = total;
            
            return listaRetorno;
        }

        public async Task<List<ProdutoModel>> ListarPorRamoAtividadeAsync(string idRamoAtividade, string ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null,
                                                                          string? IdCliente = null)
        {
            List<ProdutoModel> listaRetorno = new List<ProdutoModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<ProdutoDocument> filterCompleto = Builders<ProdutoDocument>.Filter.Where(r => r.Ativo == true);
            filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => !r.IdCliente.Equals(MongoDB.Bson.ObjectId.Parse(IdCliente)));

            filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.AnyEq("RamosAtividadesDirecionado",idRamoAtividade);

            var total = context.Produtos.Find(filterCompleto).CountDocuments();

            SortDefinition<ProdutoDocument> orderBy = null;

            if (string.IsNullOrEmpty(ordenacao) || ordenacao.ToLower() == "titulo")
            {
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");
            }
            else if (ordenacao.ToLower() == "menorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("ValorVenda");
            else if (ordenacao.ToLower() == "maiorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Descending("ValorVenda");
            else
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");

            context.Produtos.Find(filterCompleto)
                .Sort(orderBy)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                    listaRetorno.Add(x.Mapper());
                });

            if (listaRetorno != null && listaRetorno.Count > 0)
                listaRetorno[0].TotalRegistros = total;

            return listaRetorno;
        }

        public async Task<List<ProdutoModel>> ListarAsync(List<string> ids)
        {
            List<ProdutoModel> listaRetorno = new List<ProdutoModel>();
 
            //FilterDefinition<ProdutoDocument> filterCompleto = Builders<ProdutoDocument>.Filter.Empty;

            //ids.ForEach(a =>
            //{
            //    filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Eq("_id",MongoDB.Bson.ObjectId.Parse(a));
            //});


            List<MongoDB.Bson.ObjectId> listaIds = new List<ObjectId>();
            ids.ForEach(x =>
            {
                listaIds.Add(MongoDB.Bson.ObjectId.Parse(x));
            });

            var filter = Builders<ProdutoDocument>.Filter
                .In(p => p.IdProduto, listaIds);


            var total = context.Produtos.Find(filter).CountDocuments();
             
            context.Produtos.Find(filter).ToList().ForEach(x => {
                    listaRetorno.Add(x.Mapper());
                });

            return listaRetorno;
        }

        public async Task<ProdutoModel> ManterAsync(ProdutoModel model)
        {
            var document = model.Mapper();


            if (document.IdProduto == MongoDB.Bson.ObjectId.Empty)
            {
                await context.Produtos.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<ProdutoDocument>.Filter.Eq("_id", document.IdProduto);
                await context.Produtos.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.Produtos.Indexes.CreateOneAsync(
                Builders<ProdutoDocument>.IndexKeys.Ascending(x => x.IdProduto)
                );

            context.Produtos.Indexes.CreateOneAsync(
                Builders<ProdutoDocument>.IndexKeys.Ascending(x => x.CodInterno)
            );

            context.Produtos.Indexes.CreateOneAsync(
                Builders<ProdutoDocument>.IndexKeys.Ascending(x => x.IdCategoria)
            );

            context.Produtos.Indexes.CreateOneAsync(
                Builders<ProdutoDocument>.IndexKeys.Ascending(x => x.IdCliente)
            );

            return document.Mapper();
        }

        public async Task AtualizarEstoqueAsync(string idProduto, int qtBaixar)
        {

            var produto = await context.Produtos
                                 .Find(x => x.IdProduto == MongoDB.Bson.ObjectId.Parse(idProduto))
                                 .FirstOrDefaultAsync();

            produto.Estoque = produto.Estoque - qtBaixar;
            
            var filter = Builders<ProdutoDocument>.Filter.Eq("_id", produto.IdProduto);
            await context.Produtos.ReplaceOneAsync(filter, produto); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            
        }

        public async Task<ProdutoModel> ObterAsync(string id)
        {
            ObjectId idConvertido;

            if (MongoDB.Bson.ObjectId.TryParse(id, out idConvertido))
                return context.Produtos
                            .Find(x => x.IdProduto== idConvertido)
                            .FirstOrDefaultAsync().Result.Mapper();
            else
                return null;
        }
 
        public async Task<List<ProdutoModel>> ListarAtivosAsync(string titulo, string idCategoria = null, List<string> categorias = null, string? IdCliente = null, string? idRamoAtividade = null, string? ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null)
        {
            List<ProdutoModel> listaRetorno = new List<ProdutoModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<ProdutoDocument> filterCompleto = Builders<ProdutoDocument>.Filter.Empty;

            if (idCategoria != null)
            {
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => r.IdCategoria.Equals(MongoDB.Bson.ObjectId.Parse(idCategoria)))
                                                & Builders<ProdutoDocument>.Filter.Where(r => r.Ativo == true);

            }
            else
            {

                FilterDefinition<ProdutoDocument> filterInicial = Builders<ProdutoDocument>.Filter.Where(x => x.Titulo.ToLower().Contains(titulo))
                                                    & Builders<ProdutoDocument>.Filter.Where(r => r.Ativo == true);

                if(idRamoAtividade!=null)
                      filterInicial = filterInicial & Builders<ProdutoDocument>.Filter.StringIn("RamosAtividadesDirecionado", idRamoAtividade);

                List<MongoDB.Bson.ObjectId> ids = new List<MongoDB.Bson.ObjectId>();


                FilterDefinition<ProdutoDocument> filterSecundario = Builders<ProdutoDocument>.Filter.Where(x => x.Titulo.ToLower().Contains(titulo))
                                                    & Builders<ProdutoDocument>.Filter.Where(r => r.Ativo == true);


                if (categorias != null)
                {
                    foreach (var item in categorias)
                    {
                        ids.Add(MongoDB.Bson.ObjectId.Parse(item));
                    }

                    filterSecundario = filterSecundario & Builders<ProdutoDocument>.Filter.In("IdCategoria", ids.ToArray());

                }

                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Or(
                                                   filterInicial,
                                                   filterSecundario);
            }


            //Não pegar produtos do proprio cliente
            if (IdCliente != null)
                filterCompleto = filterCompleto & Builders<ProdutoDocument>.Filter.Where(r => !r.IdCliente.Equals(MongoDB.Bson.ObjectId.Parse(IdCliente)));

            var total = context.Produtos.Find(filterCompleto).CountDocuments();

            SortDefinition<ProdutoDocument> orderBy = null;

            if (string.IsNullOrEmpty(ordenacao) || ordenacao.ToLower() == "titulo")
            {
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");
            }
            else if (ordenacao.ToLower() == "menorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("ValorVenda");
            else if (ordenacao.ToLower() == "maiorvalor")
                orderBy = Builders<ProdutoDocument>.Sort.Descending("ValorVenda");
            else
                orderBy = Builders<ProdutoDocument>.Sort.Ascending("Titulo");

            context.Produtos.Find(filterCompleto)
                .Sort(orderBy)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                    listaRetorno.Add(x.Mapper());
                });

            if (listaRetorno != null && listaRetorno.Count > 0)
                listaRetorno[0].TotalRegistros = total;

            return listaRetorno;
        }
    }
}
