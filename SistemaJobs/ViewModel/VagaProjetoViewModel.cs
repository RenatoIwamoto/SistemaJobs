using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaJobs.ViewModel
{
    public class VagaProjetoViewModel
    {
        public int VagaProjetoViewModelId { get; set; }

        //VagaProjeto
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal SalarioOrcamento { get; set; }
        public int QtdVagas { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string RegimeContratacao { get; set; }
        public System.DateTime DataFinal { get; set; }
        //Cargos
        public string NomeCargo { get; set; }
        public int Status { get; set; }
        //Competencias
        public string NomeCompetencia { get; set; }

        public string[] ListaCargos { get; set; }
        public string[] ListaCompetencias { get; set; }
    }
}