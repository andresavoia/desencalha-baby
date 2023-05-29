using System.ComponentModel.DataAnnotations;

namespace Savoia.Desencalha.Domain.Models
{
    public class RamoAtividadeModel : BaseModel
    {
        public string IdRamoAtividade { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}
