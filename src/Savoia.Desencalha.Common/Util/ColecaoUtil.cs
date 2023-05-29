using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Savoia.Desencalha.Common.Util
{
    public static class ColecaoUtil
    {
        #region Métodos
        public static object RedimArray(Array Array, int Tamanho)
        {
            Type t = Array.GetType().GetElementType();
            Array nArray = Array.CreateInstance(t, Tamanho);
            Array.Copy(Array, 0, nArray, 0, Math.Min(Array.Length, Tamanho));
            return nArray;
        }

        public static void UnirValores<T, S>(T DadosEntrada, ref S DadosSaida)
        {
            if (DadosEntrada != null)
            {
                Type tipoDadosEntrada = DadosEntrada.GetType();
                Type tipoDadosSaida = DadosSaida.GetType();

                PropertyInfo[] propriedades = tipoDadosEntrada.GetProperties();
                PropertyInfo propriedadeVo = null;

                for (int i = 0; i < propriedades.Length; i++)
                {
                    propriedadeVo = tipoDadosSaida.GetProperty(propriedades[i].Name);
                    if (propriedadeVo != null && propriedadeVo.PropertyType.Name == propriedades[i].PropertyType.Name)

                        try
                        {
                            if (propriedades[i].PropertyType.Name != "List`1")
                            {

                                if (propriedadeVo.GetValue(DadosSaida, null) == null && propriedades[i].GetValue(DadosEntrada, null) != null)
                                    propriedadeVo.SetValue(DadosSaida, propriedades[i].GetValue(DadosEntrada, null), null);
                                else if (propriedadeVo.GetValue(DadosSaida, null) != null)
                                {
                                    if (propriedadeVo.GetValue(DadosSaida, null).GetType() == typeof(bool))
                                    {
                                        if ((bool)propriedadeVo.GetValue(DadosSaida, null) == false && propriedades[i].GetValue(DadosEntrada, null) != null)
                                            propriedadeVo.SetValue(DadosSaida, propriedades[i].GetValue(DadosEntrada, null), null);
                                    }
                                }
                            }
                        }
                        catch { }
                }
            }
            else
            {
                DadosSaida = default(S);
            }
        }

        public static S AtribuirValores<T, S>(T DadosEntrada)
        {
            if (DadosEntrada != null)
            {
                S DadosSaida = Activator.CreateInstance<S>();

                Type tipoDadosEntrada = DadosEntrada.GetType();
                Type tipoDadosSaida = DadosSaida.GetType();

                PropertyInfo[] propriedades = tipoDadosEntrada.GetProperties();
                PropertyInfo propriedadeVo = null;

                for (int i = 0; i < propriedades.Length; i++)
                {
                    propriedadeVo = tipoDadosSaida.GetProperty(propriedades[i].Name);
                    if (propriedadeVo != null && propriedadeVo.PropertyType.Name == propriedades[i].PropertyType.Name)
                    {
                        //Attribute attribute = Attribute.GetCustomAttribute(propriedades[i], typeof(KeyAttribute)) as KeyAttribute;

                        if (propriedades[i].PropertyType.Name != "List`1" && propriedades[i].PropertyType.Name != "IList`1")
                            propriedadeVo.SetValue(DadosSaida, propriedades[i].GetValue(DadosEntrada, null), null);
                        //else
                        //    propriedadeVo.SetValue(DadosSaida, AtribuirValoresLista<T, S>((List<T>)propriedades[i].GetValue(DadosEntrada, null)), null); //(IList)propriedades[i].GetValue(DadosEntrada, null), null);

                    }
                }

                return DadosSaida;
            }
            else
            {
                return default(S);
            }
        }

        public static List<S> AtribuirValoresLista<T, S>(List<T> listaEntrada)
        {
            List<S> retorno = new List<S>();
            if (listaEntrada != null &&
                listaEntrada is IList)
            {
                T elemento;
                S elementoSaida;

                for (int i = 0; i < ((IList)listaEntrada).Count; i++)
                {
                    elemento = (T)((IList)listaEntrada)[i];
                    elementoSaida = AtribuirValores<T, S>(elemento);
                    retorno.Add(elementoSaida);
                }
            }

            if (retorno.Count == 0)
                return null;
            else
                return retorno;
        }

        public static List<T> FiltrarLista<T>(List<T> Itens, string Texto, string NomePropriedade)
        {
            List<T> itensEncontrados = new List<T>();

            for (int i = 0; i < Itens.Count; i++)
            {
                if (StringUtil.TirarAcentos(typeof(T).GetProperty(NomePropriedade).GetValue(Itens[i], null).ToString(), true).IndexOf(Texto, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    itensEncontrados.Add(Itens[i]);
            }

            return itensEncontrados;
        }

        public static Dictionary<K, V> FiltrarLista<K, V>(Dictionary<K, V> Itens, string Texto)
        {
            Dictionary<K, V> itensEncontrados = new Dictionary<K, V>();

            foreach (KeyValuePair<K, V> kvp in Itens)
            {
                if (StringUtil.TirarAcentos(kvp.Value.ToString(), true).IndexOf(Texto, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    itensEncontrados.Add(kvp.Key, kvp.Value);

            }

            return itensEncontrados;
        }




        #endregion

    }
}
