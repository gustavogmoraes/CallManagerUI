using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorDeChamadas.Solucao.Negocio.Objetos
{
    public class Chamada
    {
        public Guid Id { get; set; }

        public string Canal { get; set; }

        public string IdDoChamador { get; set; }

        public int MaximoDeTentativas { get; set; }

        public int TempoEntreTentativas { get; set; }

        public int TempoParaAguardarChamando { get; set; }

        public string Contexto { get; set; }

        public string Extensao { get; set; }
    }
}
