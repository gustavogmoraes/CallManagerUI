using System;

namespace GerenciadorDeChamadas.Solucao.Negocio.Servicos
{
    public  class ChamadaParaExecutar
    {
        public ChamadaParaExecutar()
        {
        }

        public Guid Id { get; set; }

        public DateTime Horario { get; set; }

        public string NumeroParaLigar { get; set; }
    }
}