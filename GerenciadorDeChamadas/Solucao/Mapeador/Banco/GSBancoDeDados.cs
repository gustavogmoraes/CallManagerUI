using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Mapeador.Banco
{
    public class GSBancoDeDados : IDisposable
    {
        #region Constantes

        public const string DIRETORIO_LOCAL = @".\";

        public const string NOME_ARQUIVO_CONFIGURACOES_BANCO = "ConexaoBanco.txt";

        #endregion

        #region Propriedades

        private MySqlConnection _conexao;

        private InformacoesConexaoBanco _informacoesConexao;

        private static string _stringDeConexao;

        #endregion

        #region Construtores

        public GSBancoDeDados()
        {
            //if (_stringDeConexao == null && _informacoesConexao == null)
            //{
            //    _informacoesConexao = SessaoSistema.InformacoesConexao;
            //DefinaStringDeConexao("192.168.15.155","snep","root","sneppass");
            //}

            _stringDeConexao = "Server=192.168.15.155;Database=snep;Uid=root;Pwd=sneppass;SslMode=enabled";

            _conexao = new MySqlConnection(_stringDeConexao);
            _conexao.Open();
        }   

        #endregion

        #region Métodos

        /// <summary>
        /// Define a string de conexão com o banco de dados.
        /// </summary>
        /// <param name="urlDoServidor">A url do servidor, para local utilize '.', lembre-se de passar a string com o '@' antes para ignorar as '\'.</param>
        /// <param name="nomeDoBanco">O nome do banco.</param>
        /// <param name="login">O usuário do banco.</param>
        /// <param name="senha">A senha do banco.</param>
        private void DefinaStringDeConexao(string urlDoServidor, string nomeDoBanco, string login, string senha)
        {
            _stringDeConexao = $"Server = {urlDoServidor}; Database = {nomeDoBanco}; Uid = {login}; Pwd = {senha};";
        }

        #region Métodos de query

        /// <summary>
        /// Executa diretamente a query SQL enviada.
        /// </summary>
        /// <param name="comandoSQL">A query SQL que será executada.</param>
        public void ExecuteComando(string comandoSQL)
        {
            try
            {
                //_conexao = new MySqlConnection(_stringDeConexao);

                //Cria e define a query
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = comandoSQL;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _conexao;

                //Executa a query
                cmd.ExecuteReader();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Executa diretamente a query SQL enviada e retorna o resultado retorno tupla única.
        /// </summary>
        /// <param name="comandoSQL">A query SQL que será executada.</param>
        /// <returns>
        /// DataReader com o resultado da query.
        /// </returns>
        public dynamic ExecuteConsultaRetornoUnico(string comandoSQL, Type tipoRetorno)
        {
            try
            {
                string resultadoConsulta = string.Empty;

                //Cria e define a query
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = comandoSQL;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _conexao;

                //Executa a query
                resultadoConsulta = cmd.ExecuteScalar().ToString();

                if (String.IsNullOrEmpty(resultadoConsulta))
                    return null;

                return tipoRetorno.GetMethod("Parse", new Type[] { typeof(string) })
                                    .Invoke(null, new object[] { resultadoConsulta });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Executa diretamente a query SQL enviada e retorna o resultado.
        /// </summary>
        /// <param name="comandoSQL">A query SQL que será executada.</param>
        /// <returns>
        /// DataReader com o resultado da query.
        /// </returns>
        public DataTable ExecuteConsulta(string comandoSQL)
        {
            try
            {
                var dataTable = new DataTable();

                //Cria e define a query
                MySqlCommand cmd = new MySqlCommand();

                //Cria o reader
                MySqlDataReader reader;

                cmd.CommandText = comandoSQL;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _conexao;

                //Executa a query
                reader = cmd.ExecuteReader();

                dataTable.Load(reader);

                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion


        #region Métodos auxiliares

        public bool VerifiqueStatusDaConexao()
        {
            try
            {
                //Cria e define a query
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = "SELECT 1";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _conexao;

                //Executa a query
                var retorno = cmd.ExecuteScalar();

                if ((int)retorno == 1)
                    return true;
                else
                    return false;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Obtém o próximo código inteiro que estiver disponível na tabela desejada.
        /// </summary>
        /// <param name="tabela">A tabela desejada</param>
        /// <param name="colunaChave">A coluna chave dessa tabela</param>
        /// <returns>
        /// Inteiro com o próximo código disponível
        /// </returns>
        public int ObtenhaProximoCodigoDisponivel(string tabela, string colunaChave)
        {
            string comandoSQL = String.Format("SELECT TOP 1 {0} + 1 FROM {1} MO WHERE NOT EXISTS" +
                                                "(SELECT NULL FROM {1} MI WHERE MI.{0} = MO.{0} + 1) ORDER BY {0}",
                                                colunaChave,
                                                tabela);

            var retorno = ExecuteConsultaRetornoUnico(comandoSQL, typeof(int));

            if (retorno == null)
                return 1;

            return retorno;
        }

        #endregion

        #endregion

        #region Dispose 

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);  
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_conexao != null)
                {
                    _conexao.Close();
                    _conexao = null;
                }


                _stringDeConexao = null;

                _informacoesConexao = null;
            }
        }

        #endregion
    }
}
