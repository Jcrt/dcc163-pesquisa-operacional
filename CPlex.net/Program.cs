﻿using CPlex.net.Model;
using CPlex.net.Solver;
using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;


namespace CPlex.net
{
    class Program
    {

        private static string _inputPath = "..\\..\\data\\input.xlsx";
        private static string _outputPath = "..\\..\\data\\output.xlsx";
        private static EntradaViewModel _entradaViewModel;
        private static SaidaViewModel _saidaViewModel;
        private static readonly CPlexSolver _cplexSolver = new CPlexSolver();

        static void Main(string[] args)
        {

            Console.WriteLine("Iniciando Cplex!");

            CarregaInformacoes();
            ExecutaOtimizacao();
            GravaResultadoOtimizacao();

            Console.WriteLine("Acesse o arquivo output.xlsx para ver o resultado da otimização");
            Console.WriteLine("Aperte qualquer tecla para encerrar...");
            Console.ReadKey();
        }

        #region Leitura dos dados

        /// <summary>
        /// Carrega informações dos arquivo input.xlsx
        /// </summary>
        private static void CarregaInformacoes()
        {
            Console.WriteLine("Iniciando leitura dos dados");
            _entradaViewModel = new EntradaViewModel();

            using (var stream = File.Open(_inputPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();

                    LeDadosDaEquipe(result.Tables[0]);
                    LeDadosDosProdutos(result.Tables[0]);
                }
            }
            Console.WriteLine("Leitura dos dados encerrada");
        }

        /// <summary>
        /// Le as informações a respeito de carga horária da equipe
        /// </summary>
        /// <param name="dataSet"></param>
        private static void LeDadosDaEquipe(DataTable dataSet)
        {
            Console.WriteLine("Iniciando leitura dos dados da equipe");
            //foreach(var item in dataSet.Rows[2].ItemArray)
            for (int i = 0; i < 6; i++)
            {
                var item = dataSet.Rows[2].ItemArray[i];
                _entradaViewModel.CargaHorariaDisponivel.Add((DiaDaSemana)i+1, int.Parse(item.ToString()));
            }
            Console.WriteLine("Leitura dos dados da equipe encerrada");
        }

        /// <summary>
        /// Le as informações dos produtos a serem produzidos
        /// </summary>
        /// <param name="dataSet"></param>
        private static void LeDadosDosProdutos(DataTable dataSet)
        {

            Console.WriteLine("Iniciando leitura dos dados dos produtos");
            
            for (int i = 6; i < dataSet.Rows.Count; i++)
            {
                var produto = new ProdutoViewModel();
                produto.Nome = dataSet.Rows[i].ItemArray[0].ToString();
                produto.TaxaProducao = float.Parse(dataSet.Rows[i].ItemArray[1].ToString());
                produto.CustoRegular = float.Parse(dataSet.Rows[i].ItemArray[8].ToString());
                produto.CustoHoraExtra = float.Parse(dataSet.Rows[i].ItemArray[9].ToString());
                produto.Validade = int.Parse(dataSet.Rows[i].ItemArray[10].ToString());
                for (int j = 2; j < 8; j++)
                {
                    var item = dataSet.Rows[i].ItemArray[j];
                    produto.Demanda.Add((DiaDaSemana)j-1, int.Parse(item.ToString()));
                }
                _entradaViewModel.Produtos.Add(produto);
            }
            Console.WriteLine("Leitura dos dados dos produtos encerrada");
        }

        #endregion

        /// <summary>
        /// Executa a otimização
        /// </summary>
        private static void ExecutaOtimizacao()
        {
            _cplexSolver.Run(_entradaViewModel);
            MockSaida(); //RETIRAR ESSE MÉTODO
        }

        /// <summary>
        /// Grava o resultado da otimização em arquivo
        /// </summary>
        private static void GravaResultadoOtimizacao()
        {
            File.Delete(_outputPath);
            FileInfo saida = new FileInfo(_outputPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage pck = new ExcelPackage(saida);

            var planilhaSaida = pck.Workbook.Worksheets.Add("Saida");
            planilhaSaida.Cells["A1"].Value = "Produto";
            planilhaSaida.Cells["B1"].Value = "Produção segunda";
            planilhaSaida.Cells["C1"].Value = "Produção terça";
            planilhaSaida.Cells["D1"].Value = "Produção quarta";
            planilhaSaida.Cells["E1"].Value = "Produção quinta";
            planilhaSaida.Cells["F1"].Value = "Produção sexta";
            planilhaSaida.Cells["G1"].Value = "Produção sábado";
            planilhaSaida.Cells["A1:G1"].Style.Font.Bold = true;

            var linha = 2;
            foreach (var produto in _saidaViewModel.Produtos)
            {
                planilhaSaida.Cells["A" + linha].Value = produto.Nome;
                planilhaSaida.Cells["B" + linha].Value = produto.Producao[DiaDaSemana.Segunda];
                planilhaSaida.Cells["C" + linha].Value = produto.Producao[DiaDaSemana.Terça];
                planilhaSaida.Cells["D" + linha].Value = produto.Producao[DiaDaSemana.Quarta];
                planilhaSaida.Cells["E" + linha].Value = produto.Producao[DiaDaSemana.Quinta];
                planilhaSaida.Cells["F" + linha].Value = produto.Producao[DiaDaSemana.Sexta];
                planilhaSaida.Cells["G" + linha].Value = produto.Producao[DiaDaSemana.Sábado];

                linha++;
            }

            pck.Save();
        }

        //RETIRAR ESSE MÉTODO
        private static void MockSaida()
        {
            List<ProdutoViewModel> produtos = new List<ProdutoViewModel>();

            foreach( var produto in _entradaViewModel.Produtos)
            {
                var produtoSaida = new ProdutoViewModel()
                {
                    Nome = produto.Nome
                };

                for (int i =1; i<7; i++)
                {
                    produtoSaida.Producao.Add((DiaDaSemana)i, i*10);
                }

                produtos.Add(produtoSaida);
            }

            _saidaViewModel = new SaidaViewModel
            {
                Produtos = produtos
            };
        }
    }
}
