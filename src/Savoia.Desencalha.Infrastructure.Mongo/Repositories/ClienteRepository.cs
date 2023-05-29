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
    public class ClienteRepository : IClienteRepository
    {
        private readonly MongoDbContext context = null;

        public ClienteRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<long> ExcluirAsync(string id)
        {
            var filter = Builders<ClienteDocument>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));

            var result = context.Clientes.DeleteOne(filter);
            return result.DeletedCount;
        }
 
        public async Task<bool> ConsistirAsync(string idCliente, string codInterno, string cpfOuCnpj, string login)
        {
            List<ClienteModel> listaRetorno = new List<ClienteModel>();

            FilterDefinition<ClienteDocument> filterCompleto = Builders<ClienteDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(idCliente))
            filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Where(r => r.IdCliente != MongoDB.Bson.ObjectId.Parse(idCliente));

            if (!string.IsNullOrEmpty(codInterno))
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Eq(r => r.CodInterno, codInterno);
            else
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Or(
                                    Builders<ClienteDocument>.Filter.Eq(r => r.CpfOuCnpj, cpfOuCnpj),
                                    Builders<ClienteDocument>.Filter.Eq(r => r.Login, login));

            var total = context.Clientes.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        public async Task<bool> ConsistirAsync(string idRamoAtividade)
        {
            List<ClienteModel> listaRetorno = new List<ClienteModel>();

            FilterDefinition<ClienteDocument> filterCompleto = Builders<ClienteDocument>.Filter.Empty;

            filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Where(r => r.IdRamoAtividade == MongoDB.Bson.ObjectId.Parse(idRamoAtividade));

            var total = context.Clientes.Find(filterCompleto).CountDocuments();

            return total > 0;
        }

        public async Task<ClienteModel> ObterAsync(string id)
        {
            ObjectId idConvertido;

            if (MongoDB.Bson.ObjectId.TryParse(id, out idConvertido))
                return context.Clientes
                            .Find(x => x.IdCliente == idConvertido)
                            .FirstOrDefaultAsync().Result.Mapper();
            else
                return null;
        }

        public async Task<ClienteModel> ManterAsync(ClienteModel model)
        {
            var document = model.Mapper();


            if (document.IdCliente == MongoDB.Bson.ObjectId.Empty)
            {
                await context.Clientes.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<ClienteDocument>.Filter.Eq("_id", document.IdCliente);
                await context.Clientes.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.Clientes.Indexes.CreateOneAsync(
                Builders<ClienteDocument>.IndexKeys.Ascending(x => x.IdCliente)
                );

            context.Clientes.Indexes.CreateOneAsync(
                Builders<ClienteDocument>.IndexKeys.Ascending(x => x.CodInterno)
            );

            context.Clientes.Indexes.CreateOneAsync(
                Builders<ClienteDocument>.IndexKeys.Ascending(x => x.CodClienteStatus)
            );

            context.Clientes.Indexes.CreateOneAsync(
                Builders<ClienteDocument>.IndexKeys.Ascending(x => x.NomeOuRazao)
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

        public async Task<List<ClienteModel>> ListarAsync(string codInterno, string nomeOuRazao, int? codFreteTipo, string cpfOuCnpj, int? codClienteStatus = null, int? paginaAtual = null, int? numeroRegistros = null)
        {
            List<ClienteModel> listaRetorno = new List<ClienteModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            FilterDefinition<ClienteDocument> filterCompleto = Builders<ClienteDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(codInterno))
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Eq(r => r.CodInterno, codInterno);

            if (!string.IsNullOrEmpty(cpfOuCnpj))
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Eq(r => r.CpfOuCnpj, cpfOuCnpj);

            if (!string.IsNullOrEmpty(nomeOuRazao))
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Where(x => x.NomeOuRazao.ToLower().Contains(nomeOuRazao));

            if (codClienteStatus!=null)
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Where(r => r.CodClienteStatus.Equals((int)codClienteStatus));

            if (codFreteTipo != null)
                filterCompleto = filterCompleto & Builders<ClienteDocument>.Filter.Where(r => r.CodFreteTipo.Equals((int)codFreteTipo));

            var total = context.Clientes.Find(filterCompleto).CountDocuments();

            context.Clientes.Find(filterCompleto)
                .SortBy(x=>x.NomeOuRazao)
                .Skip(registroInicial).Limit((int)numeroRegistros).ToList().ForEach(x => {
                listaRetorno.Add(x.Mapper());
            });

            return listaRetorno;
        }

        public async Task<ClienteModel> ObterAsync(string login, string senha)
        {
            return context.Clientes
                        .Find(x => x.Login == login && x.Senha == senha)
                        .FirstOrDefaultAsync().Result.Mapper();
        }

        public async Task<long> ObterTotalStatusAsync(int codClienteStatus)
        {
            return context.Clientes.Find(Builders<ClienteDocument>.Filter.Where(r => r.CodClienteStatus.Equals((int)codClienteStatus))).CountDocumentsAsync().Result;
        }


        public async Task<ClienteModel> ObterPorLoginAsync(string login)
        {
            return context.Clientes
                        .Find(x => x.Login == login)
                        .FirstOrDefaultAsync().Result.Mapper();
        }
    }
}
