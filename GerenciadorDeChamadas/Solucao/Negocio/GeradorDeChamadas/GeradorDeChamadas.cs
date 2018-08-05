using GerenciadorDeChamadas.Solucao.Negocio.Enumeradores;
using GerenciadorDeChamadas.Solucao.Negocio.Objetos;
using GerenciadorDeChamadas.Solucao.Negocio.Servicos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Negocio.GeradorDeChamadas
{
    public class GeradorDeChamadas
    {
        public void CrieNovaChamada(
        EnumTiposDeChamada tipoDeChamada, 
        EnumTiposDeCanal tipoDoCanal,
        string canal,
        string identificacaoDoChamador,
        int maximoDeTentativas,
        int tempoEntreTentativas,
        int tempoParaAguardarChamando)
        {
            var idDaChamada = Guid.NewGuid();
            using (var servicoDeChamadasParaExecutar = new ServicoDeChamadasParaExecutar())
            {
                while (servicoDeChamadasParaExecutar.VerifiqueSeChamadaJaEstaNaFilaDeExecucao(idDaChamada))
                {
                    idDaChamada = Guid.NewGuid();
                }
            }
              
            string stringTipoDoCanal;
            switch (tipoDoCanal)
            {
                case EnumTiposDeCanal.SIP:
                    stringTipoDoCanal = "SIP";
                    break;

                default:
                    stringTipoDoCanal = "";
                    break;
            }

            string stringTipoDeChamada;
            switch (tipoDeChamada)
            {
                case EnumTiposDeChamada.COMUM:
                    stringTipoDeChamada = "Comum";
                    break;

                case EnumTiposDeChamada.AVISO_DE_BOLETO_A_VENCER:
                    stringTipoDeChamada = "AvisoDeBoleto";
                    break;

                case EnumTiposDeChamada.PESQUISA_DE_SATISFACAO:
                    stringTipoDeChamada = "PesquisaDeSatisfacao";
                    break;
    
                default:
                    stringTipoDeChamada = "Comum";
                    break;
            }

            var chamada =
                new Chamada()
                {
                    Id = idDaChamada,
                    Canal = $"{stringTipoDoCanal}/{canal}",
                    IdDoChamador = identificacaoDoChamador,
                    MaximoDeTentativas = maximoDeTentativas,
                    TempoEntreTentativas = tempoEntreTentativas,
                    TempoParaAguardarChamando = tempoParaAguardarChamando,
                    Contexto = stringTipoDeChamada,
                    Extensao = "s"
                };

            //Gravar o id na tabela de chamadas pra executar
            //esse gravar do banco, tem que ter retorno com sucesso antes de colocar o arquivo no diretorio

            CrieArquivoDeChamadaNoDiretorio(chamada);
        }

        public void CrieArquivoDeChamadaNoDiretorio(Chamada chamada)
        {
            const string DIRETORIO = "../var/spool/asterisk/outgoing";

            var nomeDoArquivo = chamada.Id.ToString().Replace("-", string.Empty);

            if (File.Exists(DIRETORIO + nomeDoArquivo))
            {
                File.Delete(DIRETORIO + nomeDoArquivo);
            }

            using (var writer = new StreamWriter(DIRETORIO + nomeDoArquivo, true))
            {
                writer.WriteLine($"Channel: {chamada.Canal}\n" +
                                 $"Callerid: {chamada.IdDoChamador}\n" +
                                 $"MaxRetries: {chamada.MaximoDeTentativas}\n" +
                                 $"RetryTime: {chamada.TempoEntreTentativas}\n" +
                                 $"WaitTime: {chamada.TempoParaAguardarChamando}\n" +
                                 $"Context: {chamada.Contexto}\n" +
                                 $"Extension: {chamada.Extensao}");
            }
        }

        public void CrieAudioNoDiretorio()
        {
            const string DIRETORIO = "/var/lib/asterisk/sounds/pt_BR";

            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                // Configuramos a saída do áudio, indicando a 
                //  qualidade do arquivo .wav 
                synth.SetOutputToWaveFile(@".\teste.wav",
                new SpeechAudioFormatInfo(32000, AudioBitsPerSample.Sixteen,
                AudioChannel.Mono));

                //Criamos o objeto SoundPlayer, responsável por “tocar” 
                // um arquivo .wav

                System.Media.SoundPlayer m_SoundPlayer =
                  new System.Media.SoundPlayer(@".\teste.wav");

                // Construímos um promptBuilder
                PromptBuilder builder = new PromptBuilder();

                // Indicamos o nome da voz (propriedade Name -> VoiceInfo ) 
                builder.StartVoice("ScanSoft Raquel_Full_22Hz");

                // Adicionamos o texto ao nosso prompt
                builder.AppendText("O Hiago é viado!");

                builder.EndVoice();

                // Speak the prompt.
                synth.Speak(builder);

                //Vamos ouvir o arquivo .wav
                m_SoundPlayer.Play();
            }
        }
    }
}
