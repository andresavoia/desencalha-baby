using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class EnderecoModel
    {
        public string IdEndereco { get; set; }
        public CidadeModel Cidade { get; set; }
        public string Bairro { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string Obs { get; set; }

    }
}
