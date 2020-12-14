using System.Collections.Generic;

namespace CPlex.net.Model
{
    class EntradaViewModel
    {
        public EntradaViewModel()
        {
            Produtos = new List<ProdutoViewModel>();
            CargaHorariaDisponivel = new Dictionary<DiaDaSemana, int>();
        }

        public List<ProdutoViewModel> Produtos { get; set; }

        public Dictionary<DiaDaSemana,int> CargaHorariaDisponivel  { get; set; }

    }
}
