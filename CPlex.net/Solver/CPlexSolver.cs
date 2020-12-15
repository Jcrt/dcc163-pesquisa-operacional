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
        private readonly Dictionary<string, double> _varArray;

        public CPlexSolver()
        {
            _cplex = new Cplex();
            _variaveis = new INumVar[1][];
            _restricoes = new IRange[1][];
            _varArray = new Dictionary<string, double>();
        }

        public void Run(EntradaViewModel entrada)
        {
            CriaVariaveis(entrada);
            AddFuncaoObjetivo(entrada);
            AddRestricoes(entrada);
        }

        void AddRestricoes(EntradaViewModel entrada)
        {
            throw new NotImplementedException();
        }

        void CriaVariaveis(EntradaViewModel entrada)
        {
            entrada.Produtos.ForEach(item => {
                _varArray.Add($"x{_varArray.Count+1}", item.GetTaxaUnidadeHora());
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
    }
}
