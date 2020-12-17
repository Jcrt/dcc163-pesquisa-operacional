using CPlex.net.Model;
using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPlex.net.Solver
{
    class CPlexSolver
    {
        private readonly Cplex _cplex;
        private readonly INumVar[][] _variaveis;
        private readonly IRange[][] _restricoes;
        private readonly IDictionary<string, int> _varArray;
        private readonly IList<IRange> _restricoesList;

        public CPlexSolver()
        {
            _cplex = new Cplex();
            _variaveis = new INumVar[1][];
            _restricoes = new IRange[1][];
            _varArray = new Dictionary<string, int>();
            _restricoesList = new List<IRange>();
        }

        public void Run(EntradaViewModel entrada)
        {
            CriaVariaveis(entrada);
            AddRestricoes(entrada);
            //AddFuncaoObjetivo(entrada);
        }

        void AddRestricoes(EntradaViewModel entrada)
        {
            AddRestricaoDemandaMinima(entrada);
        }

        void CriaVariaveis(EntradaViewModel entrada)
        {
            entrada.Produtos.ForEach(item => {

                foreach(var diaSemana in Enum.GetValues(typeof(DiaDaSemana)))
                {
                    var produtoHR = item.GetNomeVariavel((DiaDaSemana)diaSemana, false);
                    var produtoHE = item.GetNomeVariavel((DiaDaSemana)diaSemana, true);

                    _varArray.Add(produtoHR, _varArray.Count);
                    _varArray.Add(produtoHE, _varArray.Count);
                }
            });

            var varArray = _cplex.NumVarArray(
                _varArray.Count,
                _varArray.Select(item => 0.0d).ToArray(),
                _varArray.Select(item => double.MaxValue).ToArray(),
                _varArray.Keys.ToArray()
            );

            _variaveis[1] = varArray;
        }

        void AddFuncaoObjetivo(EntradaViewModel entrada)
        {
            double[] calculoFuncaoObjetivo = { };
            _cplex.AddMinimize(_cplex.ScalProd(_variaveis[1], calculoFuncaoObjetivo));
        }

        void AddRestricaoDemandaMinima(EntradaViewModel entrada)
        {
            entrada.Produtos.ForEach(produto =>
            {
                foreach (var diaSemana in Enum.GetValues(typeof(DiaDaSemana)))
                {
                    var produtoHR = produto.GetNomeVariavel((DiaDaSemana)diaSemana, false);
                    var produtoHE = produto.GetNomeVariavel((DiaDaSemana)diaSemana, true);
                    var demanda = produto.Demanda[(DiaDaSemana)diaSemana];
                    var equacao = _cplex.Prod(
                        produto.GetTaxaUnidadeHora(),
                        _variaveis[0][_varArray[produtoHR]],
                        _variaveis[0][_varArray[produtoHE]]
                    );

                    var restricao = _cplex.AddGe(equacao, demanda, $"RestricaoProducaoMinima_{produto.GetNomeLimpo()}_{diaSemana}");
                    _restricoesList.Add(restricao);
                }
            });
        }
    }
}
