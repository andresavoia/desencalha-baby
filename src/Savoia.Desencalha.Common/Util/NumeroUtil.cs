using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Common.Util
{
    public class NumeroUtil
    {
        #region Métodos
        /// <summary>
        /// Caso o numero seja menor que 10 ele retorna um 0 a frente do mesmo.
        /// </summary>
        /// <param name="numero"></param>
        /// <returns>Número Formatado</returns>
        public static string Zero(int Numero)
        {
            if (Numero < 10)
                return "0" + Numero.ToString();
            else
                return Numero.ToString();
        }

        public static string ObterValorBrasil(decimal valor, bool mostrarSimbolo = false)
        {
            if (mostrarSimbolo)
                return string.Format(new CultureInfo("pt-BR"), "{0:C}", valor);
            else
                return string.Format(new CultureInfo("pt-BR"), "{0:N2}", valor);

        }

        public static string ObterValorBrasil(float valor, bool mostrarSimbolo = false)
        {
            if(mostrarSimbolo)
                return string.Format(new CultureInfo("pt-BR"),"{0:C}", valor);
            else
                return string.Format(new CultureInfo("pt-BR"), "{0:N2}", valor);

        }

        public static string ObterValorBrasil(double valor, bool mostrarSimbolo = false)
        {
            if (mostrarSimbolo)
                return string.Format(new CultureInfo("pt-BR"), "{0:C}", valor);
            else
                return string.Format(new CultureInfo("pt-BR"), "{0:N2}", valor);

        }

        public static bool ValidarNumero(string Valor)
        {
            decimal resultado;
            return Decimal.TryParse(Valor, NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowThousands |
                NumberStyles.AllowLeadingSign | NumberStyles.AllowParentheses,
                CultureInfo.CurrentCulture, out resultado);
        }

        public static int? ToIntNullable(string Numero)
        {
            try
            {
                if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                    return null;
                else
                    return int.Parse(Numero);
            }
            catch { return 0; }
        }

        public static int? ToIntNullable(object Numero)
        {
            if (Numero == null || Numero.ToString().Trim() == string.Empty)
                return null;
            else
                return Convert.ToInt32(Numero);
        }

        public static int ToInt(string Numero)
        {
            try
            {
                if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                    return 0;
                else
                    return int.Parse(Numero);
            }
            catch { return 0; }
        }

        public static int ToInt(object Numero)
        {
            if (Numero == null || Numero.ToString().Trim() == string.Empty)
                return 0;
            else
                return Convert.ToInt32(Numero);
        }

        public static long? ToLongNullable(string Numero)
        {
            if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                return null;
            else
                return long.Parse(Numero);
        }

        public static long ToLong(string Numero)
        {
            if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                return 0;
            else
                return long.Parse(Numero);
        }

        public static decimal? ToDecimalNullable(string Numero)
        {
            if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                return null;
            else
                return decimal.Parse(Numero);
        }

        public static decimal ToDecimal(string Numero)
        {
            if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                return 0;
            else
                return decimal.Parse(Numero);
        }

        public static decimal ToDouble(string Valor)
        {
            CultureInfo Cultura = new CultureInfo("pt-BR", false);

            if (string.IsNullOrEmpty(Valor))
                return 0;

            try
            {
                return Decimal.Parse(Valor, Cultura.NumberFormat);
            }
            catch
            {
                return 0;
            }

        }


        public static decimal? FloatToDecimalNullable(float Valor)
        {
            decimal Retorno = 0;
            CultureInfo Cultura = new CultureInfo("pt-BR", false);

            if (string.IsNullOrEmpty(Valor.ToString()))
                return Retorno;

            try
            {
                Retorno = Decimal.Parse(Valor.ToString(), Cultura.NumberFormat);
            }
            catch
            {
                Retorno = 0;
            }
            return Retorno;

        }

        public static decimal FloatToDecimal(float Valor)
        {
            decimal Retorno = 0;
            CultureInfo Cultura = new CultureInfo("pt-BR", false);

            if (string.IsNullOrEmpty(Valor.ToString()))
                return Retorno;

            try
            {
                Retorno = Decimal.Parse(Valor.ToString(), Cultura.NumberFormat);
            }
            catch
            {
                Retorno = 0;
            }
            return Retorno;

        }

        public static string LongToString(long Numero)
        {
            if (Numero <= 0)
                return string.Empty;
            else
                return Numero.ToString();
        }

        public static string TratarStringNumerico(string Numero)
        {
            if (string.IsNullOrEmpty(Numero))
                return string.Empty;
            else
                return Numero;
        }

        public static Int64? ToInt64(string Numero)
        {
            if (Numero == null || Numero.Trim() == string.Empty || !ValidarNumero(Numero))
                return null;
            else
                return Int64.Parse(Numero);
        }

        public static object ValidarParametro(int Valor)
        {
            if (Valor == 0)
                return (object)DBNull.Value;
            else
                return Valor;
        }

        public static object ValidarParametro(int? Valor)
        {
            return Valor.HasValue ? Valor : (object)DBNull.Value;
        }

        public static object ValidarParametro(long? Valor)
        {
            return Valor.HasValue ? Valor : (object)DBNull.Value;
        }

        public static object ValidarParametro(decimal Valor)
        {
            if (Valor == 0)
                return (object)DBNull.Value;
            else
                return Valor;
        }

        public static object ValidarParametro(long Valor)
        {
            if (Valor == 0)
                return (object)DBNull.Value;
            else
                return Valor;
        }

        public static object ValidarParametro(decimal? Valor)
        {
            return Valor.HasValue ? Valor : (object)DBNull.Value;
        }

        public static object ValidarParametro(byte? Valor)
        {
            return Valor.HasValue ? Valor : (object)DBNull.Value;
        }

        public static string RetornarDoubleSql(string DoubleBrasil)
        {
            return DoubleBrasil.Replace(".", "").Replace(",", ".");
        }


        public static decimal ToDecimal(object Numero)
        {
            if (Numero == null || Numero.ToString().Trim() == "" || !ValidarNumero(Numero.ToString()))
                return 0;
            else
                return Decimal.Parse(Numero.ToString());
        }

        public static byte ToByte(object Valor)
        {
            if (Valor == null || Valor.ToString().Trim() == "" || !ValidarNumero(Valor.ToString()))
                return 0;
            else
                return Byte.Parse(Valor.ToString());
        }


        public enum TipoValorExtenso
        {
            Monetario,
            Porcentagem,
            Decimal
        }

        public static string ObterValorPorExtenso(double Valor, TipoValorExtenso tipoValorExtenso)
        {
            decimal valorEscrever = new decimal(Valor);

            return ConverterExtenso(valorEscrever, tipoValorExtenso);
        }

        // O método toExtenso recebe um valor do tipo decimal
        public static string ConverterExtenso(decimal valor, TipoValorExtenso tipoValorExtenso)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                throw new ArgumentOutOfRangeException("Valor não suportado pelo sistema. Valor: " + valor);

            string strValor = String.Empty;
            strValor = valor.ToString("000000000000000.00#");
            //strValor = valor.ToString("{0:0.00#}");
            string valor_por_extenso = string.Empty;
            int qtdCasasDecimais =
                strValor.Substring(strValor.IndexOf(',') + 1, strValor.Length - (strValor.IndexOf(',') + 1)).Length;
            bool existemValoresAposDecimal = Convert.ToInt32(strValor.Substring(16, qtdCasasDecimais)) > 0;

            for (int i = 0; i <= 15; i += 3)
            {
                var parte = strValor.Substring(i, 3);
                // se parte contém vírgula, pega a substring com base na quantidade de casas decimais.
                if (parte.Contains(","))
                {
                    parte = strValor.Substring(i + 1, qtdCasasDecimais);
                }
                valor_por_extenso += escreva_parte(Convert.ToDecimal(parte));
                if (i == 0 & valor_por_extenso != string.Empty)
                {
                    if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                        valor_por_extenso += " TRILHÃO" +
                                             ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                    else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                        valor_por_extenso += " TRILHÕES" +
                                             ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                }
                else if (i == 3 & valor_por_extenso != string.Empty)
                {
                    if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                        valor_por_extenso += " BILHÃO" +
                                             ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                    else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                        valor_por_extenso += " BILHÕES" +
                                             ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                }
                else if (i == 6 & valor_por_extenso != string.Empty)
                {
                    if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                        valor_por_extenso += " MILHÃO" +
                                             ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                    else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                        valor_por_extenso += " MILHÕES" +
                                             ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0)
                                                  ? " E "
                                                  : string.Empty);
                }
                else if (i == 9 & valor_por_extenso != string.Empty)
                    if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                        valor_por_extenso += " MIL" +
                                             ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0)
                                                  ? " E "
                                                  : string.Empty);

                if (i == 12)
                {
                    if (valor_por_extenso.Length > 8)
                        if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" |
                            valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                            valor_por_extenso += " DE";
                        else if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" |
                                 valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES" |
                                 valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                            valor_por_extenso += " DE";
                        else if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                            valor_por_extenso += " DE";

                    if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                    {
                        switch (tipoValorExtenso)
                        {
                            case TipoValorExtenso.Monetario:
                                valor_por_extenso += " REAL";
                                break;
                            case TipoValorExtenso.Porcentagem:
                                if (existemValoresAposDecimal == false)
                                    valor_por_extenso += " PORCENTO";
                                break;
                            case TipoValorExtenso.Decimal:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("tipoValorExtenso");
                        }
                    }

                    else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                    {
                        switch (tipoValorExtenso)
                        {
                            case TipoValorExtenso.Monetario:
                                valor_por_extenso += " REAIS";
                                break;
                            case TipoValorExtenso.Porcentagem:
                                if (existemValoresAposDecimal == false)
                                    valor_por_extenso += " PORCENTO";
                                break;
                            case TipoValorExtenso.Decimal:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("tipoValorExtenso");
                        }
                    }

                    if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                    {
                        switch (tipoValorExtenso)
                        {
                            case TipoValorExtenso.Monetario:
                                valor_por_extenso += " E ";
                                break;
                            case TipoValorExtenso.Porcentagem:
                                valor_por_extenso += " VÍRGULA ";
                                break;
                            case TipoValorExtenso.Decimal:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("tipoValorExtenso");
                        }
                    }
                }

                if (i == 15)
                    if (Convert.ToInt32(strValor.Substring(16, qtdCasasDecimais)) == 1)
                    {
                        switch (tipoValorExtenso)
                        {
                            case TipoValorExtenso.Monetario:
                                valor_por_extenso += " CENTAVO";
                                break;
                            case TipoValorExtenso.Porcentagem:
                                valor_por_extenso += " CENTAVO";
                                break;
                            case TipoValorExtenso.Decimal:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("tipoValorExtenso");
                        }
                    }

                    else if (Convert.ToInt32(strValor.Substring(16, qtdCasasDecimais)) > 1)
                    {
                        switch (tipoValorExtenso)
                        {
                            case TipoValorExtenso.Monetario:
                                valor_por_extenso += " CENTAVOS";
                                break;
                            case TipoValorExtenso.Porcentagem:
                                valor_por_extenso += " PORCENTO";
                                break;
                            case TipoValorExtenso.Decimal:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("tipoValorExtenso");
                        }
                    }
            }
            return valor_por_extenso;
        }

        private static string escreva_parte(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));

                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";

                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";

                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";

                return montagem;
            }
        }

        public static bool ValidarCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool ValidarCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }

    #endregion

}
