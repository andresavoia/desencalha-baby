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
    public class RamoAtividadeRepository : IRamoAtividadeRepository
    {
        private readonly MongoDbContext context = null;

        public RamoAtividadeRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<long> ExcluirAsync(string id)
        {
            var filter = Builders<RamoAtividadeDocument>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));

            var result = context.RamoAtividades.DeleteOne(filter);
            return result.DeletedCount;
        }

        public async Task<List<RamoAtividadeModel>> ListarAsync(string idRamoAtividade, string titulo, bool? ativo = null, int? paginaAtual = null, int? numeroRegistros = null)
        {
            List<RamoAtividadeModel> listaRetorno = new List<RamoAtividadeModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<RamoAtividadeDocument> filterCompleto = Builders<RamoAtividadeDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(idRamoAtividade))
                filterCompleto = filterCompleto & Builders<RamoAtividadeDocument>.Filter.Eq(r => r.IdRamoAtividade, MongoDB.Bson.ObjectId.Parse(idRamoAtividade));

            if (!string.IsNullOrEmpty(titulo))
                filterCompleto = filterCompleto & Builders<RamoAtividadeDocument>.Filter.Where(x => x.Titulo.ToLower().Contains(titulo));

            if (ativo!=null)
                filterCompleto = filterCompleto & Builders<RamoAtividadeDocument>.Filter.Where(x => x.Ativo == ativo);

            var total = context.RamoAtividades.Find(filterCompleto).CountDocuments();

            context.RamoAtividades
                .Find(filterCompleto)
                .SortBy(x=>x.Titulo)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                listaRetorno.Add(x.Mapper());
            });

            return listaRetorno;
        }

        public async Task<bool> ConsistirAsync(string idRamoAtividade, string titulo)
        {
            List<RamoAtividadeModel> listaRetorno = new List<RamoAtividadeModel>();

            FilterDefinition<RamoAtividadeDocument> filterCompleto = Builders<RamoAtividadeDocument>.Filter.Empty;

            if(!string.IsNullOrEmpty(idRamoAtividade))
               filterCompleto = filterCompleto & Builders<RamoAtividadeDocument>.Filter.Where(r => r.IdRamoAtividade != MongoDB.Bson.ObjectId.Parse(idRamoAtividade));

            filterCompleto = filterCompleto & Builders<RamoAtividadeDocument>.Filter.Eq(r => r.Titulo, titulo);

            var total = context.RamoAtividades.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        public async Task<RamoAtividadeModel> ObterAsync(string id)
        {
            ObjectId idConvertido;

            if (MongoDB.Bson.ObjectId.TryParse(id, out idConvertido))
                return context.RamoAtividades
                            .Find(x => x.IdRamoAtividade == idConvertido)
                            .FirstOrDefaultAsync().Result.Mapper();
            else
                return null;
        }

        public async Task<RamoAtividadeModel> ManterAsync(RamoAtividadeModel model)
        {
            var document = model.Mapper();


            if (document.IdRamoAtividade == MongoDB.Bson.ObjectId.Empty)
            {
                await context.RamoAtividades.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<RamoAtividadeDocument>.Filter.Eq("_id", document.IdRamoAtividade);
                await context.RamoAtividades.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.RamoAtividades.Indexes.CreateOneAsync(
                Builders<RamoAtividadeDocument>.IndexKeys.Ascending(x => x.IdRamoAtividade)
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
