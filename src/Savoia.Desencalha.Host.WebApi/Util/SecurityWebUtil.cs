using Microsoft.IdentityModel.Tokens;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Util
{
    public static class SecurityWebUtil
    {
        public static string GerarToken(IConfiguration configuration, 
            string nome,
            string idUsuario,
            string codUsuarioPerfil,
            string login,
            DateTime dataExpiracao,
            bool? isVendedor = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["AppSettings:jwt-key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, nome),
                    new Claim(ClaimTypes.Role, codUsuarioPerfil),
                    new Claim(ClaimTypes.Email, login),
                    new Claim(ClaimTypes.Sid, idUsuario),
                    new Claim(ClaimTypes.AuthorizationDecision, isVendedor.HasValue ? isVendedor.Value.ToString() : string.Empty)
                }),
                Expires = dataExpiracao,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
