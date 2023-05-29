using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class CategoriaModel : BaseModel
    {
        public string IdCategoria { get; set; }
        public string CodInterno { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}
