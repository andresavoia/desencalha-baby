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
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly MongoDbContext context = null;

        public CategoriaRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<long> ExcluirAsync(string id)
        {
            var filter = Builders<CategoriaDocument>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));

            var result = context.Categorias.DeleteOne(filter);
            return result.DeletedCount;
        }

        public async Task<List<CategoriaModel>> ListarAsync(string idCategoria, string codInterno, string titulo, bool? ativo = null, int? paginaAtual = null, int? numeroRegistros = null)
        {
            List<CategoriaModel> listaRetorno = new List<CategoriaModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<CategoriaDocument> filterCompleto = Builders<CategoriaDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(idCategoria))
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Eq(r => r.IdCategoria, MongoDB.Bson.ObjectId.Parse(idCategoria));

            if (!string.IsNullOrEmpty(codInterno))
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Eq(r => r.CodInterno, codInterno);

            if (!string.IsNullOrEmpty(titulo))
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Where(x => x.Titulo.ToLower().Contains(titulo));

            if (ativo!=null)
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Where(x => x.Ativo == ativo);

            var total = context.Categorias.Find(filterCompleto).CountDocuments();

            context.Categorias
                .Find(filterCompleto)
                .SortBy(x=>x.Titulo)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                listaRetorno.Add(x.Mapper());
            });

            return listaRetorno;
        }

        public async Task<bool> ConsistirAsync(string idCategoria, string codInterno, string titulo)
        {
            List<CategoriaModel> listaRetorno = new List<CategoriaModel>();

            FilterDefinition<CategoriaDocument> filterCompleto = Builders<CategoriaDocument>.Filter.Empty;

            if(!string.IsNullOrEmpty(idCategoria))
               filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Where(r => r.IdCategoria != MongoDB.Bson.ObjectId.Parse(idCategoria));

            if (string.IsNullOrEmpty(codInterno))
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Eq(r => r.Titulo, titulo);
            else
                filterCompleto = filterCompleto & Builders<CategoriaDocument>.Filter.Or(Builders<CategoriaDocument>.Filter.Eq(r => r.CodInterno, codInterno), Builders<CategoriaDocument>.Filter.Eq(r => r.Titulo, titulo));

            var total = context.Categorias.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        public async Task<CategoriaModel> ObterAsync(string id)
        {
            ObjectId idConvertido;

            if (MongoDB.Bson.ObjectId.TryParse(id, out idConvertido))
                return context.Categorias
                            .Find(x => x.IdCategoria == idConvertido)
                            .FirstOrDefaultAsync().Result.Mapper();
            else
                return null;
        }

        public async Task<CategoriaModel> ManterAsync(CategoriaModel model)
        {
            var document = model.Mapper();


            if (document.IdCategoria == MongoDB.Bson.ObjectId.Empty)
            {
                await context.Categorias.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<CategoriaDocument>.Filter.Eq("_id", document.IdCategoria);
                await context.Categorias.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.Categorias.Indexes.CreateOneAsync(
                Builders<CategoriaDocument>.IndexKeys.Ascending(x => x.IdCategoria)
                );

            //     coll.Indexes.CreateOneAsync(Builders<MyEntity>.IndexKeys.Combine(
            //Builders<MyEntity>.IndexKeys.Ascending("list.a"),
            //Builders<MyEntity>.IndexKeys.Ascending("list.b")), new CreateIndexOptions()
            //{
            //    Unique = true,
            //    Sparse = true
            //});

            return document.Mapper();

        }

    }
}
