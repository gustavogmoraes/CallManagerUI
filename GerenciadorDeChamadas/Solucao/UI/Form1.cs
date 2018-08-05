using GerenciadorDeChamadas.Solucao.Negocio.GeradorDeChamadas;
using GerenciadorDeChamadas.Solucao.Negocio.Objetos;
using GerenciadorDeChamadas.Solucao.Negocio.Servicos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerenciadorDeChamadas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void RecarregarTabela()
        {
            dataGridView1.Rows.Clear();

            using (var servico = new ServicoDeChamadasParaExecutar())
            {
                var listaDeChamadas = servico.ConsulteTodasAsChamadasParaExecutar();

                foreach (var chamada in listaDeChamadas)
                {
                    dataGridView1.Rows.Add(chamada.Key.Id,
                                           chamada.Value.NomeDoCliente,
                                           chamada.Value.Descricao,
                                           chamada.Value.DataDoVencimento);
                }
            }

            dataGridView1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var textoParaChamadaDeBoleto =
                new TextoParaBoleto()
                {
                    NomeDoCliente = txtNomeDoCliente.Text.Trim(),
                    Descricao = txtDescricao.Text.Trim(),
                    DataDoVencimento = dateTimePicker1.Value
                };

            using (var servico = new ServicoDeChamadasParaExecutar())
            {
                servico.InserirNovaChamada(txtNumero.Text.Trim(), textoParaChamadaDeBoleto);
            }

            RecarregarTabela();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //RecarregarTabela();
        }
    }
}
