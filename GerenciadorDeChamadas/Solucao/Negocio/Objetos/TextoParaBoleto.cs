using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Negocio.Objetos
{
    public class TextoParaBoleto
    {
        public string NomeDoCliente { get; set; }

        public string Descricao { get; set; }

        public DateTime DataDoVencimento { get; set; }
    }
}
