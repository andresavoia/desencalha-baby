using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Validators;
using System.Text;
using Savoia.Desencalha.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(
    x =>
    {
        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        x.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();

//Configurando JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:jwt-key"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

builder.Services.AddMemoryCache();


#region Adicionando Dependencias

//Add Dependecies
builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddTransient<ICategoriaValidator, CategoriaValidator>();
builder.Services.AddTransient<IClienteValidator, ClienteValidator>();
builder.Services.AddTransient<IProdutoValidator, ProdutoValidator>();
builder.Services.AddTransient<IPedidoValidator, PedidoValidator>();
builder.Services.AddTransient<IClienteValidator, ClienteValidator>();
builder.Services.AddTransient<IUsuarioValidator, UsuarioValidator>();
builder.Services.AddTransient<IRamoAtividadeValidator, RamoAtividadeValidator>();
builder.Services.AddTransient<IFreteValidator, FreteValidator>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Allow For All Clients
app.UseCors("AllowAll");

//tem q ter isso pra funcionar a pasta www com as imagens
app.UseStaticFiles();

//Add Guid CorrelatoionID Response
app.UseCorrelationId(); //Middleware caseiro

app.UseHttpsRedirection();

//Add JWT Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
