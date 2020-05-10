using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaJobs.ViewModels
{
    public class VmUsuarioConversa
    {
        public string Usuario { get; set; }
        public int? IdConversa { get; set; }
        public int? IdRemetenteInicial { get; set; }
        public int IdDestinatario { get; set; }
        public int NaoLida { get; set; }
    }
}