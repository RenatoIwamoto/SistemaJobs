using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaJobs.Models
{
    public class EmpresaModel
    {
        [Key]
        public int IdEmpresa { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [StringLength(14, /*MinimumLength = 14,*/ ErrorMessage = "O CNPJ deve ter 14 caracteres")]
        public string CNPJ { get; set; }

        [Required]
        //[DataType(DataType.PhoneNumber)]
        [StringLength(11, MinimumLength = 10)]
        public string Telefone { get; set; }

        [Required]
        public string Email { get; set; }

        public string Estado { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Cidade { get; set; }

        [Required]
        public string Usuario { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 8)]
        public string Senha { get; set; }
    }
}