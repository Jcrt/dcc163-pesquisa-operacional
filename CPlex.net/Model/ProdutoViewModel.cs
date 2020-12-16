using System;
using System.Collections.Generic;

namespace CPlex.net.Model
{
    class ProdutoViewModel
    {
        public ProdutoViewModel()
        {
            Demanda = new Dictionary<DiaDaSemana, int>();
            Producao = new Dictionary<DiaDaSemana, int>();
        }

        public string Nome { get; set; }

        public float TaxaProducao { get; set; }

        public Dictionary<DiaDaSemana, int> Demanda { get; set; }

        public int Validade { get; set; }

        public float CustoRegular { get; set; }

        public float CustoHoraExtra { get; set; }

        public double GetTaxaUnidadeHora()
        {
            if (TaxaProducao <= 0.0f)
                throw new InvalidOperationException($"A propriedade {nameof(TaxaProducao)} não pode ser 0");

            return 1 / TaxaProducao;
        }

        public Dictionary<DiaDaSemana, int> Producao { get; set; }
    }
}
