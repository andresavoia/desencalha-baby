using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Savoia.Desencalha.Common.Util.ConstantesUtil;

namespace Savoia.Desencalha.Domain.Models
{
    public class InfoVendedor
    {
        public virtual string IdCliente { get; set; }
        public virtual string NomeClienteVendedor { get; set; }
        public virtual FreteEstadoModel FreteEstado { get; set; }
        public virtual int CodFreteTipo { get; set; }
        public virtual double ValorFreteFixo{ get; set; }
        public virtual int DiasPrazoEntregaFixo { get; set; }
        public virtual int DiasPrazoEntregaEscolhido
        {
            get 
            {
                if (CodFreteTipo == (int)FreteTipo.ValorFixo)
                {
                    return DiasPrazoEntregaFixo;
                }
                else
                {
                    if (FreteEstado?.DiasPrazoEntrega == null) return 0;

                    return FreteEstado.DiasPrazoEntrega.Value;
                }
            }
        }
    }
}
