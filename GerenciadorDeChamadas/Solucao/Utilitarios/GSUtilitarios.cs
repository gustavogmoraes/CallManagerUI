using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Negocio
{
    public class GSUtilitarios
    {
        #region Propriedades

        public static readonly Dictionary<Type, string> DicionarioTipoDadosParaBancoDeDados;

        #endregion


        #region BancoDeDados

        static GSUtilitarios()
        {
            DicionarioTipoDadosParaBancoDeDados = new Dictionary<Type, string>()
            {
                {typeof(string), "NVARCHAR"},
                {typeof(int), "INT"},
                {typeof(DateTime), "DATETIME2"},
                {typeof(decimal), "DECIMAL"},
                {typeof(Guid), "NVARCHAR" }
            };
        }

        #endregion


        #region Utilitários p/ Banco de Dados

        /// <summary>
        /// Converte o nome do tipo de dado do SGBD para um tipo equivalente no sistema.
        /// </summary>
        /// <param name="tipoDadoBanco">Nome do tipo de dado a ser convertido</param>
        /// <returns>Retorna um tipo de dado equivalente no sistema.</returns>
        public static Type ConvertaTipoDadosAplicacaoBanco(string tipoDadoBanco)
        {
            return DicionarioTipoDadosParaBancoDeDados.FirstOrDefault(x => x.Value == tipoDadoBanco).Key;
        }

        public static bool ConvertaValorBooleano(string valorNoBanco)
        {
            return valorNoBanco == "S" ? true : false;
        }

        public static string ConvertaValorBooleano(bool booleano)
        {
            return booleano ? "S" : "N";
        }

        #endregion


        #region Utilitários p/ Criação de Objetos

        /// <summary>
		/// Cria uma lista do tipo passado
		/// </summary>
		/// <param name="tipo">Tipo dos elementos da lista</param>
		/// <returns>Retorna uma lista do tipo passado.</returns>
		public static IList CrieLista(Type tipo)
        {
            Type tipoGenericoLista = typeof(List<>).MakeGenericType(tipo);

            return (IList)Activator.CreateInstance(tipoGenericoLista);
        }

        #endregion


        #region Utilitários p/ Formatação

        /// <summary>
        /// Converte o DateTime passado para uma string compatível para ser inserida no SGBD.
        /// </summary>
        /// <param name="data">DateTime a ser convertido</param>
        /// <returns>Retorna uma string com o DateTime formatado em "yyyy-MM-dd HH:mm:SS".</returns>
        public static string FormateDateTimePtBrParaBD(DateTime data)
        {
            return String.Format("{0}-{1}-{2} {3}:{4}:{5}",
                                 data.Year,
                                 data.Month.ToString().PadLeft(2, '0'), //Garante que terá o zero à esquerda
                                 data.Day.ToString().PadLeft(2, '0'),
                                 data.Hour.ToString().PadLeft(2, '0'),
                                 data.Minute.ToString().PadLeft(2, '0'),
                                 data.Second.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// Converte a string passada no formato "yyyy-MM-dd HH:mm:SS" para um DateTime.
        /// </summary>
        /// <param name="dataBD">String a ser convertida</param>
        /// <returns>Retorna um DateTime com as informações passadas na string.</returns>
        public static DateTime FormateDateTimePtBrParaBD(string dataBD)
        {
            DateTime data = DateTime.ParseExact(dataBD, "yyyy-MM-dd HH:mm:SS", new CultureInfo("pt-BR"));

            return data;
        }

        public static string FormateDecimalParaStringMoedaReal(decimal valor)
        {
            return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", valor).Replace("R$ ", String.Empty);
        }
        #endregion


        #region Utilitários p/ Manipulação de Strings

        /// <summary>
        /// Obtem o valor(em string) dentro de outras duas strings.
        /// </summary>
        /// <param name="dado">String total</param>
        /// <param name="primeiraString">Primeira string delimitadora</param>
        /// <param name="ultimaString">Segunda string delimitadora</param>
        /// <returns>Retorna o dado obtido.</returns>
        public static string ObtenhaValorEntreStrings(string dado, string primeiraString, string ultimaString)
        {
            string retorno;

            int posicao1 = dado.IndexOf(primeiraString) + primeiraString.Length;
            int posicao2 = dado.IndexOf(ultimaString);

            retorno = dado.Substring(posicao1, posicao2 - posicao1);

            return retorno;
        }

        #endregion


        #region Utilitários p/ Propriedades

        public static string FormateDecimalParaMoeda(decimal precoDeVenda)
        {
            var cultura = new CultureInfo("pt-BR");

            return string.Format(cultura, "{0:C}", precoDeVenda).Remove(0, 2);
        }

        public static List<PropertyInfo> EncontrePropriedadeMarcadaComAtributo(Type tipo, Type tipoDoAtributo)
        {
            return tipo.GetProperties()
                       .Where(x => Attribute.IsDefined(x, tipoDoAtributo)).ToList();
        }

        #endregion


        #region Utilitários p/ Tipos

        /// <summary>
		/// Obtem o tipo da lista.
		/// </summary>
		/// <param name="lista">Lista</param>
		/// <returns>Retorna o tipo da lista.</returns>
		public static Type ObtenhaTipoLista<T>(List<T> lista)
        {
            return typeof(T);
        }

        /// <summary>
        /// Obtem o tipo da lista.
        /// </summary>
        /// <param name="lista">Lista</param>
        /// <returns>Retorna o tipo da lista.</returns>
        public static Type ObtenhaTipoListaPropriedade(PropertyInfo propriedade)
        {
            //return propriedade.PropertyType.GenericTypeArguments[0]; --> Esse também funciona, dá na mesma, doido

            return propriedade.PropertyType.GetProperty("Item").PropertyType;
        }

        /// <summary>
        /// Obtem o tipo de um tipo enumerado
        /// </summary>
        /// <param name="Enumeravel">Tipo enumerado</param>
        /// <returns>Retorna o tipo do tipo enumerado.</returns>
        public static Type ObtenhaTipoEnumerado<T>(IEnumerable<T> Enumeravel)
        {
            return typeof(T);
        }

        #endregion


        #region Utilitários p/ Verificação

        /// <summary>
        /// Verifica se o tipo passado é "BuiltIn", em outras palavras, se ele é "primitivo" no .NET, não foi criado pelo programador.
        /// </summary>
        /// <param name="tipo">Tipo de dado a ser avaliado</param>
        /// <returns>Retorna um valor booleano respondendo se o tipo é "BuiltIn."</returns>
        public static bool ChequeSeTipoEhBuiltIn(Type tipo)
        {
            if (tipo.Namespace == "System" || tipo.Namespace.StartsWith("System") || tipo.Module.ScopeName == "CommonLanguageRuntimeLibrary")
                return true;
            else
                return false;
        }

        /// <summary>
		/// Verifica se o tipo de dado passado é uma lista de qualquer tipo.
		/// </summary>
		/// <param name="tipo">Tipo de dado a ser avaliado</param>
		/// <returns>Retorna um booleano da validação feita.</returns>
		public static bool VerifiqueSeTipoEhLista(Type tipo)
        {
            return (tipo.IsGenericType && (tipo.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static bool EhDigitoOuPonto(char caracter)
        {
            if (char.IsDigit(caracter) || char.IsPunctuation(caracter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
