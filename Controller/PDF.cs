using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCertificados.Controller
{
    public class PDF
    {
        public string Caminho { get; set; }
        public string Nome { get; set; }
        public string Formato { get; set; }

        public bool Assinado { get; set; }
    }
}
