using System;
using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class BaseModel
    {
        public string UsuarioCadastro { get; set; }
        public string UsuarioAlteracao { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }
    }
}
