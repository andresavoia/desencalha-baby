using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Savoia.Desencalha.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savoia.Desencalha.Infrastructure.Mongo.Common
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database = null;

        public MongoDbContext(IOptions<MongoSettingsOptions> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<UsuarioDocument> Usuarios
        {
            get
            {
                return _database.GetCollection<UsuarioDocument>("usuario");
            }

        }


        public IMongoCollection<RamoAtividadeDocument> RamoAtividades
        {
            get
            {
                return _database.GetCollection<RamoAtividadeDocument>("ramo-atividade");
            }

        }

        public IMongoCollection<CategoriaDocument> Categorias
        {
            get
            {
                return _database.GetCollection<CategoriaDocument>("categoria");
            }

        }

        public IMongoCollection<CidadeDocument> Cidades
        {
            get
            {
                return _database.GetCollection<CidadeDocument>("cidade");
            }
        }

        public IMongoCollection<ClienteDocument> Clientes
        {
            get
            {
                return _database.GetCollection<ClienteDocument>("cliente");
            }
        }

        public IMongoCollection<EstadoDocument> Estados
        {
            get
            {
                return _database.GetCollection<EstadoDocument>("estado");
            }
        }


        public IMongoCollection<PedidoDocument> Pedidos
        {
            get
            {
                return _database.GetCollection<PedidoDocument>("pedido");
            }
        }

        public IMongoCollection<ProdutoDocument> Produtos
        {
            get
            {
                return _database.GetCollection<ProdutoDocument>("produto");
            }
        }

        public IMongoCollection<FreteEstadoDocument> FreteEstado
        {
            get
            {
                return _database.GetCollection<FreteEstadoDocument>("frete-estado");
            }
        }

        public IMongoCollection<CarrinhoDocument> Carrinho
        {
            get
            {
                return _database.GetCollection<CarrinhoDocument>("carrinho");
            }
        }

        public IMongoCollection<PedidoVendedorDocument> PedidoVendedor
        {
            get
            {
                return _database.GetCollection<PedidoVendedorDocument>("pedido-vendedor");
            }
        }

    }
}
