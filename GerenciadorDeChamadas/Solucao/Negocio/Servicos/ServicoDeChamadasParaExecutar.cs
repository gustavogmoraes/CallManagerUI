using GerenciadorDeChamadas.Solucao.Mapeador.Banco;
using GerenciadorDeChamadas.Solucao.Negocio.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Negocio.Servicos
{
    public class ServicoDeChamadasParaExecutar : IDisposable
    {
        const string TABELA = "MBOL_CHAMADAS_PARA_PROC";
        const string COLUNAS = "ID, NUMERO, HORARIO";

        const string TABELA_TEXTOS = "MBOL_TEXTOS_BOLETO";
        const string COLUNAS_TEXTOS = "ID, NOME_CLIENTE, DESCRICAO, DATA_VENCIMENTO";

        public bool VerifiqueSeChamadaJaEstaNaFilaDeExecucao(Guid idDaChamada)
        {
            return false;
        }

        public Dictionary<ChamadaParaExecutar, TextoParaBoleto> ConsulteTodasAsChamadasParaExecutar()
        {
            var consultaSQLChamada = $"SELECT {COLUNAS} FROM {TABELA}";

            var dicionarioRetorno = new Dictionary<ChamadaParaExecutar, TextoParaBoleto>();
            using (var bancoDeDados = new GSBancoDeDados())
            {
                var resultado = bancoDeDados.ExecuteConsulta(consultaSQLChamada);
                foreach(DataRow linha in resultado.Rows)
                {
                    var chamada =
                        new ChamadaParaExecutar()
                        {
                            Id = new Guid(linha["ID"].ToString()),
                            NumeroParaLigar = linha["NUMERO"].ToString(),
                            Horario = (DateTime)linha["HORARIO"]
                        };

                    var consultaSQLTexto = $"SELECT {COLUNAS_TEXTOS} FROM {TABELA_TEXTOS} WHERE ID = '{chamada.Id}';";
                    var resultadoTexto = bancoDeDados.ExecuteConsulta(consultaSQLTexto);
                    var texto =
                        new TextoParaBoleto()
                        {
                            NomeDoCliente = resultadoTexto.Rows[0]["NOME_CLIENTE"].ToString(),
                            Descricao = resultadoTexto.Rows[0]["DESCRICAO"].ToString(),
                            DataDoVencimento = (DateTime)resultadoTexto.Rows[0]["DATA_VENCIMENTO"]
                        };
                    dicionarioRetorno.Add(chamada, texto);
                }
            }

            return dicionarioRetorno;
        }

        public void InserirNovaChamada(string numeroParaLigar, TextoParaBoleto texto)
        {
            var idDaChamada = Guid.NewGuid();

            var comandoSQLChamada = $"INSERT INTO {TABELA_TEXTOS}({COLUNAS}) " +
                                    $"VALUES ('{idDaChamada}', " +
                                    $"{numeroParaLigar}, " +
                                    $"CAST ('{DateTime.Now}' AS DATETIME);";

            var comandoSQLTexto = $"INSERT INTO {TABELA_TEXTOS}({COLUNAS_TEXTOS}) " +
                                  $"VALUES ('{idDaChamada}', " +
                                  $"'{texto.NomeDoCliente}', " +
                                  $" '{texto.Descricao}', " +
                                  $" '{ConvertaDataParaMesFalado(texto.DataDoVencimento)}');";

            using (var bancoDeDados = new GSBancoDeDados())
            {
                bancoDeDados.ExecuteComando(comandoSQLChamada);
                bancoDeDados.ExecuteComando(comandoSQLTexto);
            }
        }

        private string ConvertaDataParaMesFalado(DateTime data)
        {
            string nomeDoMes = data.ToString("MMMM", CultureInfo.CreateSpecificCulture("pt-BR"));

            return $"{data.Day} de {nomeDoMes} de {data.Year}";
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServicoDeChamadasParaExecutar() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
