using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCertificados.Entidades
{
    public class TCertificado
    {
        public int PK_Certificado { get; set; }
        public string NomeCertificado { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte[] ChavePrivada { get; set; }
        public string ChavePublica { get; set; }
        public string Emissor { get; set; }
        public string EmissorTipoO { get; set; }
        public DateTime? DataValidade { get; set; }
        public byte[] Certificado { get; set; }
    }
}
