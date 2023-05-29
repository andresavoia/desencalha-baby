using System;
using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class ClienteModel
    {

        public string IdCliente { get; set; }
        public string CodInterno { get; set; }
        public string IdRamoAtividade { get; set; }
        public int? CodClienteStatus{ get; set; }
        public int? CodFreteTipo { get; set; }
        public virtual string TipoPessoa { get; set; }
        public virtual string NomeOuRazao { get; set; }
        public virtual string ApelidoOuFantasia { get; set; }
        public virtual DateTime? DataNascOuFundacao { get; set; }
        public virtual string CpfOuCnpj { get; set; }
        public virtual string RgOuInscricao { get; set; }
        public virtual string EmailPrincipal { get; set; }
        public virtual string EmailAlternativo { get; set; }
        public virtual string Telefone1 { get; set; }
        public virtual string Telefone2 { get; set; }
        public virtual string Telefone3 { get; set; }
        public virtual string Contato { get; set; }
        public virtual string Obs { get; set; }
        public virtual string CampoAbertoSistema { get; set; }
        public string UsuarioCadastro { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string UsuarioAutorizacao { get; set; }
        public DateTime? DataCadastroSite { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataAutorizacao { get; set; }
        public virtual double? ValorFreteFixo { get; set; }

        public virtual int? DiasPrazoEntregaFixo { get; set; }
        public virtual EnderecoModel EnderecoCobranca { get; set; }
        public virtual EnderecoModel EnderecoEntrega { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }

        public string Salt { get; set; }

        public virtual string[] Categorias { get; set; }

        public virtual string TipoPessoaDesc
        {
            get
            {
                if (TipoPessoa == "PF") return "Pessoa Fisica"; else return "Pessoa Juridica";
            }
        }

    }
}
