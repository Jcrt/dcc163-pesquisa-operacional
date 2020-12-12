using System.Collections.Generic;

namespace CPlex.net.Model
{
    class EntradaViewModel
    {
        public List<ProdutoViewModel> Produtos { get; set; }

        public Dictionary<DiaDaSemana,int> CargaHorariaDisponivel  { get; set; }

    }
}
