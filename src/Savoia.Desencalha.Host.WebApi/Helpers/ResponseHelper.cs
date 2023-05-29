using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;

namespace Savoia.Desencalha.Host.WebApi.Helpers
{
    public static class ResponseHelper
    {

        public static T GetRequestIsNullResponse<T>() where T : BaseResponse, new()
        {
            var response = new T
            {
                Valido = false,
                Mensagens = new List<MensagemSistema>() { new MensagemSistema() { Mensagem = MessageResource.COMUM_REQUEST_INVALIDO } }
            };
            return response;
        }

        public static T GetResponseErrorValidation<T>(string campo, string mensagem) where T : BaseResponse, new()
        {
            return new T
            {
                Valido = false,
                Mensagens = new List<MensagemSistema>() { new MensagemSistema() {  Campo = campo, Mensagem = mensagem} }
            };

        }

        public static T GetResponseErrorValidation<T>(MensagemSistema message) where T : BaseResponse, new()
        {
            return new T
            {
                Valido = false,
                Mensagens = new List<MensagemSistema>() {message}
            };

        }

        public static T GetResponseErrorValidation<T>(List<MensagemSistema> messages) where T : BaseResponse, new()
        {
            return new T
            {
                Valido = false,
                Mensagens = messages
            };

        }

        public static T GetResponseError<T>(Exception ex) where T : BaseResponse, new() => GetResponseError<T>(string.Empty, ex);

        public static T GetResponseError<T>(string message, Exception ex = null) where T : BaseResponse, new()
        {
            var exception = ex?.InnerException ?? ex;
            if (exception != null)
            {
                if (!string.IsNullOrWhiteSpace(message))
                    message = $"{message}. {exception.ToString()}";
                else
                    message = exception.ToString();
            }

            return new T
            {
                Valido = false,
                Mensagens = new List<MensagemSistema>() { new MensagemSistema() { Mensagem = message } }
            };

        }
    }
}
