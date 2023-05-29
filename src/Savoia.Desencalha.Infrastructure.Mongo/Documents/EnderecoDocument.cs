

namespace Savoia.Desencalha.Infrastructure.Mongo.Documents
{
    public class EnderecoDocument
    {
        public string IdEndereco { get; set; }
        public CidadeDocument Cidade { get; set; }
        public string Bairro { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string Obs { get; set; }

    }
}
