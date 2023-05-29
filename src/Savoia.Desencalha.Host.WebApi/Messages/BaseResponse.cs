using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Savoia.Desencalha.Host.WebApi.Messages
{
    [DataContract(Namespace = "http://www.savoia-ecommerce.com.br/messages/types/")]
    public class BaseResponse
    {
        private bool _valido = true;
        private List<MensagemSistema> _mensagens;

        [DataMember]
        public bool Valido {
            get {
                return _valido;
            }
            set
            {
                this._valido = value;
            }
        }
        [DataMember]
        public List<MensagemSistema> Mensagens { get {

                if (_mensagens == null) _mensagens = new List<MensagemSistema>();

                return _mensagens;
            }
            set {
                _mensagens = value;
            }
        }

    }

    public class MensagemSistema
    {
        public string Mensagem { get; set; }
        public string Campo { get; set; }
    }

}
