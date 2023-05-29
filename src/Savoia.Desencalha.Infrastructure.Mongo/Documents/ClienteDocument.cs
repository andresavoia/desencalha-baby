using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class ClienteDocument
    {

        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId IdCliente { get; set; }
        public ObjectId IdRamoAtividade { get; set; }

        public string CodInterno { get; set; }
        public int ? CodClienteStatus{ get; set; }
        public int ? CodFreteTipo { get; set; }
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
        public virtual double? ValorFreteFixo { get; set; }
        public virtual int? DiasPrazoEntregaFixo { get; set; }
        public string UsuarioCadastro { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string UsuarioAutorizacao { get; set; }
        public DateTime? DataCadastroSite { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataAutorizacao { get; set; }
        public virtual EnderecoDocument EnderecoCobranca { get; set; }
        public virtual EnderecoDocument EnderecoEntrega { get; set; }
        public virtual string Login { get; set; }
        public virtual string Senha { get; set; }
        public virtual string Salt { get; set; }

        public virtual string[] Categorias { get; set; }

    }
}
