using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaJobs.ViewModel
{
    public class VagaProjetoViewModel
    {
        public int VagaProjetoViewModelId { get; set; }

        //VagaProjeto
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; }

        [Required]
        public decimal SalarioOrcamento { get; set; }

        [Required]
        public int QtdVagas { get; set; }

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Estado { get; set; }

        [Required]
        [StringLength(50)]
        public string RegimeContratacao { get; set; }

        [Required]
        public System.DateTime DataFinal { get; set; }
        //Cargos

        public string NomeCargo { get; set; }

        [Required]
        public int Status { get; set; }
        //Competencias

        public string NomeCompetencia { get; set; }

        public string[] ListaCargos { get; set; }

        public string[] ListaCompetencias { get; set; }
    }
}