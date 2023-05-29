using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class UsuarioModel
    {
        [Key]
        public string IdUsuario { get; set; }
        public string CodUsuarioTipo { get; set; }
        public string Nome { get; set; }
        public string Login{ get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Salt { get; set; }
        public bool Ativo { get; set; }
    }
}
