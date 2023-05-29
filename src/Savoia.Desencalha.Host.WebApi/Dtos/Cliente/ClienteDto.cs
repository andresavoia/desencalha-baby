using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Dtos.Cliente
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/dtos/cliente/types/")]
    public class ClienteDto
    {
        [DataMember]
        public string IdCliente { get; set; }

        [DataMember]
        public string CodInterno { get; set; }
        [DataMember]
        public string IdRamoAtividade { get; set; }

        [DataMember]
        public int ?CodClienteStatus { get; set; }
        [DataMember]
        public int ? CodFreteTipo{ get; set; }
        [DataMember]
        public virtual string TipoPessoa { get; set; }
        [DataMember]
        public virtual string TipoPessoaDesc { get; set; }
        [DataMember]
        public virtual string NomeOuRazao { get; set; }
        [DataMember]
        public virtual string ApelidoOuFantasia { get; set; }
        [DataMember]
        public virtual DateTime? DataNascOuFundacao { get; set; }
        [DataMember]
        public virtual string CpfOuCnpj { get; set; }
        [DataMember]
        public virtual string RgOuInscricao { get; set; }
        [DataMember]
        public virtual string EmailPrincipal { get; set; }
        [DataMember]
        public virtual string EmailAlternativo { get; set; }
        [DataMember]
        public virtual string Telefone1 { get; set; }
        [DataMember]
        public virtual string Telefone2 { get; set; }
        [DataMember]
        public virtual string Telefone3 { get; set; }
        [DataMember]
        public virtual string Contato { get; set; }
        [DataMember]
        public virtual string Obs { get; set; }
        [DataMember]
        public virtual string Login { get; set; }
        [DataMember]
        public virtual string CampoAbertoSistema { get; set; }
        [DataMember]
        public string UsuarioCadastro { get; set; }
        [DataMember]
        public string UsuarioAlteracao { get; set; }
        [DataMember]
        public string UsuarioAutorizacao { get; set; }
        [DataMember]
        public DateTime? DataCadastroSite { get; set; }
        [DataMember]
        public DateTime? DataAlteracao { get; set; }
        [DataMember]
        public DateTime? DataAutorizacao { get; set; }

        [DataMember]
        public virtual double? ValorFreteFixo { get; set; }
        [DataMember]
        public virtual int? DiasPrazoEntregaFixo { get; set; }

        [DataMember]
        public virtual EnderecoDto EnderecoCobranca { get; set; }
        [DataMember]
        public virtual EnderecoDto EnderecoEntrega { get; set; }


        [DataMember]
        public string ClienteStatus { get; set; } //Preeenchido no controler
        [DataMember]
        public string FreteTipo { get; set; } //Preeenchido no controler
        [DataMember]
        public string[] Categorias { get; set; } //Preeenchido no controler

    }
}
