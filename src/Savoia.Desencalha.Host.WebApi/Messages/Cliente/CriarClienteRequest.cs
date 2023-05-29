using Savoia.Desencalha.Host.WebApi.Dtos;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages.Cliente
{ 
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/Cliente/types/")]
    public class CriarClienteRequest
    {
        [DataMember]
        public string TipoPessoa { get; set; }
        [DataMember]
        public string NomeOuRazao { get; set; }
        [DataMember]
        public string IdRamoAtividade { get; set; }
        [DataMember]
        public string ApelidoOuFantasia { get; set; }
        [DataMember]
        public DateTime? DataNascOuFundacao { get; set; }
        [DataMember]
        public string CpfOuCnpj { get; set; }
        [DataMember]
        public string RgOuInscricao { get; set; }
        [DataMember]
        public string EmailPrincipal { get; set; }
        [DataMember]
        public string EmailAlternativo { get; set; }
        [DataMember]
        public string Telefone1 { get; set; }
        [DataMember]
        public string Telefone2 { get; set; }
        [DataMember]
        public string Telefone3 { get; set; }
        [DataMember]
        public string Contato { get; set; }
        [DataMember]
        public string Obs { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Senha { get; set; }
        [DataMember]
        public string SenhaConfirmar { get; set; }
        [DataMember(Name = "EnderecoCobranca")]
        public EnderecoDto EnderecoCobranca { get; set; }
        [DataMember(Name = "EnderecoEntrega")]
        public EnderecoDto EnderecoEntrega { get; set; }

        [DataMember]
        public virtual string[] Categorias { get; set; }
    }
}
