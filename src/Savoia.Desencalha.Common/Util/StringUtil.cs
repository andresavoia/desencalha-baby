using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Common.Util
{
    public class StringUtil
    {
        public enum TipoMascaraTexto { Telefone };

        public static string RetornarNulo(string texto)
        {
            return (texto == string.Empty ? null : texto);
        }

        public static string Right(string texto, int numCaracteres)
        {
            if (texto.Length - numCaracteres <= 0)
                return texto;
            else
                return texto.Substring(texto.Length - numCaracteres);
        }

        public static string RemoveDiacritics(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool ValidarEmail(string email)
        {
            Regex rg = new Regex(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");

            if (rg.IsMatch(email))
                return true;
            else
                return false;
        }

        public static string MascararTexto(string Palavra, string Mascara)
        {
            string Saida = string.Empty;

            if (Palavra != null)
            {
                if (Palavra != string.Empty)
                {

                    int QuantMaskItem = 0;

                    for (int c = 0; c < Mascara.Length; c++)
                        if (Mascara[c] == '#')
                            QuantMaskItem++;

                    if (Palavra.Length < QuantMaskItem)
                        while (Palavra.Length < QuantMaskItem)
                            Palavra = " " + Palavra;

                    int iP = 0;

                    for (int i = 0; i < Mascara.Length; i++)
                    {
                        if (Mascara[i] == '#')
                        {
                            Saida += Palavra[iP].ToString();
                            iP++;
                        }
                        else
                        {
                            Saida += Mascara[i].ToString();
                        }
                    }
                }
            }

            return Saida;
        }

        public static string MascararTexto(string Palavra, TipoMascaraTexto Tipo)
        {
            string Saida = string.Empty;
            string Mascara = string.Empty;

            if (string.IsNullOrEmpty(Palavra))
                return string.Empty;

            if (Tipo.Equals(TipoMascaraTexto.Telefone)) {
                if (Palavra.Length <= 11)
                    Mascara = "(##) ####-####";
                else
                    Mascara = "(##) #####-####";

            }
            else
                throw new Exception("Tipo não tratado");


            if (Palavra != null)
            {
                if (Palavra != string.Empty)
                {

                    int QuantMaskItem = 0;

                    for (int c = 0; c < Mascara.Length; c++)
                        if (Mascara[c] == '#')
                            QuantMaskItem++;

                    if (Palavra.Length < QuantMaskItem)
                        while (Palavra.Length < QuantMaskItem)
                            Palavra = " " + Palavra;

                    int iP = 0;

                    for (int i = 0; i < Mascara.Length; i++)
                    {
                        if (Mascara[i] == '#')
                        {
                            Saida += Palavra[iP].ToString();
                            iP++;
                        }
                        else
                        {
                            Saida += Mascara[i].ToString();
                        }
                    }
                }
            }

            return Saida;
        }

        public static string LimparCnpjOuCpf(string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                return valor.Replace(".", "").Replace(@"/", "").Replace("-", "");
            else
                return string.Empty;

        }

        public static string LimparTelefone(string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                return valor.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ","");
            else
                return string.Empty;

        }

        public static string TirarAcentos(string texto)
        {
            return TirarAcentos(texto, false);
        }

        public static string TirarAcentos(string texto, bool SomenteMinuscula)
        {

            string comAcento = "°áàâãéèêíóòôõúùûüç" + (!SomenteMinuscula ? "ÁÀÂÃÉÈÊÍÓÒÔÕÚÙÛÜÇ" : "");
            string semAcento = "oaaaaeeeioooouuuuc" + (!SomenteMinuscula ? "AAAAEEEIOOOOUUUUC" : "");

            System.Text.StringBuilder sb = new System.Text.StringBuilder(texto);

            for (int i = 0; i < comAcento.Length; i++)
            {
                sb.Replace(comAcento[i], semAcento[i]);
            }

            //string input = sb.ToString();
            //string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
            //string replacement = " ";
            //Regex rgx = new Regex(pattern);
            //string result = rgx.Replace(input, replacement);

            //return result;

            return sb.ToString();
        }


        #region CodificarUrl/DecodificarUrl

        /// <summary>
        /// Codifica String
        /// </summary>
        /// <param name="p_Entrada">String de Entrada</param>
        /// <returns>String Codificada</returns>
        public static string CodificarUrl(string p_Entrada)
        {
            string strAux = "";

            for (int i = 0; i < p_Entrada.Length; i++)
            {
                string strHex = ((int)p_Entrada[i]).ToString("X");

                if (strHex.Length < 2)
                    strHex = "0" + strHex;

                strAux = strAux + strHex;
            }

            return strAux;
        }

        public static string DecodificarUrl(object p_Entrada)
        {
            return DecodificarUrl(p_Entrada.ToString());
        }
        /// <summary>
        /// Decodifica String
        /// </summary>
        /// <param name="p_Entrada">String de Entrada</param>
        /// <returns>String Decodificada</returns>
        public static string DecodificarUrl(string p_Entrada)
        {
            string retorno = "";

            if (string.IsNullOrWhiteSpace(p_Entrada))
                return null;

            for (int i = 0; i < p_Entrada.Length; i += 2)
            {
                string strAux = "";

                strAux = p_Entrada[i].ToString() + p_Entrada[i + 1].ToString();

                char strAux2 = Convert.ToChar(Convert.ToInt32(strAux, 16));

                retorno += strAux2;
            }

            return retorno;
        }
        #endregion

    }
}
