using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using Savoia.Desencalha.Infrastructure.Mongo.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Infrastructure.Mongo.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly MongoDbContext context = null;

        public UsuarioRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }

        public async Task<long> ExcluirAsync(string id)
        {
            var filter = Builders<UsuarioDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var result = context.Usuarios.DeleteOne(filter);
            return result.DeletedCount;
        }

        public async Task<List<UsuarioModel>> ListarAsync(int? paginaAtual = null, int? numeroRegistros = null)
        {
            List<UsuarioModel> listaRetorno = new List<UsuarioModel>();

            int registroInicial = 0;
            if (numeroRegistros == 0 || numeroRegistros == null) numeroRegistros = ConstantesUtil.PAGE_SIZE_DEFAULT;
            if (paginaAtual == null || paginaAtual == 0) paginaAtual = 1;

            if (paginaAtual == 1)
                registroInicial = 0;
            else
                registroInicial = (((int)paginaAtual) - 1) * (int)numeroRegistros;

            var query = context.Usuarios.AsQueryable();

            //if (!string.IsNullOrWhiteSpace(filter.Titulo))
            //{
            //    query.Where(x => x.Titulo.ToLower().Contains(filter.Titulo));
            //}

            query.OrderBy(x => x.Nome);

            var total = (int)query.Count();

            query.Skip(registroInicial).Take((int)numeroRegistros).ToList().ForEach(x => {
                listaRetorno.Add(x.Mapper());
            });

            return listaRetorno; 
        }

        public async Task<UsuarioModel> ObterAsync(string id)
        {
            //var collection = databaseFacade.CollectionMongo<UsuarioDocument>(collectionName);

            //var filter = Builders<UsuarioDocument>.Filter.Eq("_id", ObjectId.Parse(idUsuario.ToString()));

            //return await Task.Run(() =>
            //{
            //    var result = collection.Find(filter).FirstOrDefault();
            //    return result.Mapper();
            //});

            return context.Usuarios
                            .Find(x => x.IdUsuario == MongoDB.Bson.ObjectId.Parse(id))
                            .FirstOrDefaultAsync().Result.Mapper();
        }

        public async Task<UsuarioModel> ObterPorLoginAsync(string login)
        {
            return context.Usuarios
                            .Find(x => x.Login == login)
                            .FirstOrDefaultAsync().Result.Mapper();
        }

        public async Task<UsuarioModel> ManterAsync(UsuarioModel model)
        {
            var document = model.Mapper();


            if (document.IdUsuario == MongoDB.Bson.ObjectId.Empty)
                await context.Usuarios.InsertOneAsync(document);
            else
            {
                var filter = Builders<UsuarioDocument>.Filter.Eq("_id", document.IdUsuario);
                await context.Usuarios.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.Usuarios.Indexes.CreateOneAsync(
                Builders<UsuarioDocument>.IndexKeys.Ascending(x => x.IdUsuario)
                );

        
            return document.Mapper();

        }

        public async Task<UsuarioModel> ObterAsync(string login, string senha)
        {
            return context.Usuarios 
                .Find(x => x.Login == login && x.Senha == senha)
                .FirstOrDefaultAsync().Result.Mapper();
        }
    }
}
