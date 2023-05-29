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
    public class FreteEstadoRepository : IFreteEstadoRepository
    {
        private readonly MongoDbContext context = null;

        public FreteEstadoRepository(IOptions<MongoSettingsOptions> settings)
        {
            context = new MongoDbContext(settings);
        }
         

        public async Task<List<FreteEstadoModel>> ListarAsync(string idCliente)
        {
            List<FreteEstadoModel> listaRetorno = new List<FreteEstadoModel>();

            context.FreteEstado.FindSync<FreteEstadoDocument>(x => x.IdCliente == MongoDB.Bson.ObjectId.Parse(idCliente)).
                ToList().ForEach(x =>
                {
                    listaRetorno.Add(new FreteEstadoModel()
                    {
                        IdFreteEstado = x.IdFreteEstado.ToString(),
                        Valor = x.Valor,
                        UF = x.UF,
                        ValorPedidoFreteGratis = x.ValorPedidoFreteGratis,
                        DiasPrazoEntrega = x.DiasPrazoEntrega
                    });
                });

            return listaRetorno;
        }

        public async Task<FreteEstadoModel> ManterAsync(FreteEstadoModel model)
        {
            var document = new FreteEstadoDocument()
            {
                IdFreteEstado = (string.IsNullOrEmpty(model.IdFreteEstado) ? MongoDB.Bson.ObjectId.Empty : MongoDB.Bson.ObjectId.Parse(model.IdFreteEstado)),
                UF = model.UF,
                Valor = model.Valor,
                ValorPedidoFreteGratis = model.ValorPedidoFreteGratis,
                DiasPrazoEntrega = model.DiasPrazoEntrega,
                IdCliente = MongoDB.Bson.ObjectId.Parse(model.IdCliente)
            };


            if (document.IdFreteEstado == MongoDB.Bson.ObjectId.Empty)
            {
                await context.FreteEstado.InsertOneAsync(document);
            }
            else
            {
                var filter = Builders<FreteEstadoDocument>.Filter.Eq("_id", document.IdFreteEstado);
                await context.FreteEstado.ReplaceOneAsync(filter, document); //o option que insere automaticamente, estava com erro ao inserir o Id sequencial 
            }

            //Criando indices
            context.FreteEstado.Indexes.CreateOneAsync(
                Builders<FreteEstadoDocument>.IndexKeys.Ascending(x => x.UF)
                );

            return new FreteEstadoModel()
            {
                IdFreteEstado = document.IdFreteEstado.ToString(),
                UF = document.UF,
                Valor = document.Valor
            };

        }

    }
}
