using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Savoia.Desencalha.Common.Util
{
    public class CriptografiaUtil
    {
        /// <summary>
        /// Chave de Criptografia Teor
        /// </summary>
        private const string CHAVE = "827948237$$$fdsdfdsfkj2EWEDF30*&^&%^&!%@^%!%OU)PPNY2WN-V'3XR=$?LWK+(B0,67?@@SIMWT:->-`;W_TJ47]Y^A1XF&@H4AU[IZB9_%V;$G6RMK0&*E*Q(U$.D>@(;?`+LHWYS?]X,G(&`F7EB4SQI.H]4F??MP+4]+O?Z$%T2,;NVTC70&P`E5?SJ`W?0LWIF23&<C7=X[9(U?O6E-_JE?=BD^^E-CB9KJO:[/;A;E*57^AYEQ9*D[]INPK)";

        public static string DecriptarPadrao(string Entrada)
        {
            return Decriptar(Entrada, CHAVE);
        }

        public static string EncriptarPadrao(string Entrada)
        {
            return Encriptar(Entrada, CHAVE);
        }

        /// <summary>
        /// Descripta a String de Entrada
        /// </summary>
        /// <param name="Entrada">String de Entrada</param>
        /// <returns>String Decriptada</returns>
        public static string Decriptar(string Entrada, string Chave)
        {
            byte[] _chave = { };
            byte[] modelo = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] _entrada = new byte[Entrada.Length];
            try
            {
                _chave = Encoding.UTF8.GetBytes(Chave.Substring(0, 8));
                DESCryptoServiceProvider servico = new DESCryptoServiceProvider();
                _entrada = Convert.FromBase64String(Entrada);
                MemoryStream memoria = new MemoryStream();
                CryptoStream stream = new CryptoStream(memoria, servico.CreateDecryptor(_chave, modelo), CryptoStreamMode.Write);
                stream.Write(_entrada, 0, _entrada.Length);
                stream.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(memoria.ToArray());
            }
            catch
            {
                return (string.Empty);
            }
        }

        /// <summary>
        /// Encripta a String de Entrada
        /// </summary>
        /// <param name="Entrada">String de Entrada</param>
        /// <returns>String Encriptada</returns>
        public static string Encriptar(string Entrada, string Chave)
        {
            byte[] _chave = { };
            byte[] modelo = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] _entrada;

            try
            {
                _chave = Encoding.UTF8.GetBytes(Chave.Substring(0, 8));
                DESCryptoServiceProvider servico = new DESCryptoServiceProvider();
                _entrada = Encoding.UTF8.GetBytes(Entrada);
                MemoryStream memoria = new MemoryStream();
                CryptoStream stream = new CryptoStream(memoria, servico.CreateEncryptor(_chave, modelo), CryptoStreamMode.Write);
                stream.Write(_entrada, 0, _entrada.Length);
                stream.FlushFinalBlock();
                return Convert.ToBase64String(memoria.ToArray());
            }
            catch
            {
                return (string.Empty);
            }
        }

        public static string CriptografarSenha(string senha, string salt)
        {
            try
            {
                HashAlgorithm _algoritmo = SHA512.Create();
                var encodedValue = Encoding.UTF8.GetBytes(senha+salt);
                var encryptedPassword = _algoritmo.ComputeHash(encodedValue);
                var sb = new StringBuilder();
                foreach (var caracter in encryptedPassword)
                {
                    sb.Append(caracter.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao criptografar senha", e);
            }
        }

        public static bool ValidarSenha(string senhaDigitada, string salt, string hashSenha)
        {
            try
            {
                HashAlgorithm _algoritmo = SHA512.Create();

                if (string.IsNullOrEmpty(senhaDigitada))
                    throw new NullReferenceException("Senha em branco");

                var encryptedPassword = _algoritmo.ComputeHash(Encoding.UTF8.GetBytes(senhaDigitada+salt));

                var sb = new StringBuilder();
                foreach (var caractere in encryptedPassword)
                {
                    sb.Append(caractere.ToString("X2"));
                }

                return sb.ToString().ToLower() == hashSenha;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao verificar a senha", e);
            }
        }

        public static string GerarSalt()
        {
            return GerarSalt(25);
        }

        public static string GerarSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

    }
}
