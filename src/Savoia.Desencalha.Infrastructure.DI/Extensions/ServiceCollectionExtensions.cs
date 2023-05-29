using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Infrastructure.Email;
using Savoia.Desencalha.Infrastructure.Mongo.Common;
using Savoia.Desencalha.Infrastructure.Mongo.Repositories;

namespace Savoia.Desencalha.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterConfiguration(services, configuration);
            RegisterService(services);
            RegisterServiceDomain(services);
            RegisterInfrastructure(services, configuration);

            return services;
        }

        public static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            //MongoConfig
            services.Configure<MongoSettingsOptions>(options =>
            {
                options.ConnectionString
                    = configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database
                    = configuration.GetSection("MongoConnection:Database").Value;
            });

        }


        public static void RegisterInfrastructure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IFreteEstadoRepository, FreteEstadoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IRamoAtividadeRepository, RamoAtividadeRepository>();

        }

        public static void RegisterService(IServiceCollection services)
        {
 
        }

        public static void RegisterServiceDomain(IServiceCollection services)
        {
        }
 
        
    }
}
