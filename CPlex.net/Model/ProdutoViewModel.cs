using System.Collections.Generic;

namespace CPlex.net.Model
{
    class ProdutoViewModel
    {
        public string Nome { get; set; }

        public float TaxaProducao { get; set; }

        public Dictionary<DiaDaSemana, int> Demanda { get; set; }

        public int Validade { get; set; }

        public float CustoRegular { get; set; }
        public float CustoHoraExtra { get; set; }


    }
}
